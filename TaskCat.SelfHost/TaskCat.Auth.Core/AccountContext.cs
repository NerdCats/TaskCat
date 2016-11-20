﻿namespace TaskCat.Account.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Entity.Identity;
    using Microsoft.AspNet.Identity;
    using Data.Model.Identity;
    using MongoDB.Driver;
    using Data.Entity;
    using Data.Model.Identity.Registration;
    using Data.Model.Identity.Profile;
    using System.Net.Http;
    using Data.Model.Identity.Response;
    using Data.Model.Operation;
    using Data.Model;
    using Common.Exceptions;
    using Common.Model.Pagination;
    using Utility;
    using Common.Utility;
    using Common.Email;
    using Common.Storage;
    using Common.Model.Storage;
    using Common.Db;
    using Model;
    using Lib.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using MongoDB.Bson;

    public class AccountContext : IAccountContext
    {
        // FIXME: direct dbContext usage in repo.. Should I?
        private readonly IDbContext dbContext;
        private readonly AccountManager accountManager;
        private readonly IBlobService blobService;
        private readonly IEmailService mailService;
        private readonly IServiceBusClient serviceBusClient;

        public AccountContext(
            IDbContext dbContext,
            IEmailService mailService,
            AccountManager accountManager,
            IBlobService blobService,
            IServiceBusClient serviceBusClient = null)
        {
            this.dbContext = dbContext;
            this.accountManager = accountManager;
            this.blobService = blobService;
            this.mailService = mailService;
            this.serviceBusClient = serviceBusClient;
        }

        // Register is always used for someone not in the database, only first time User or first time Asset use this method
        public async Task<AccountResult> RegisterUser(RegistrationModelBase model)
        {
            UserProfile profile;
            User user = null;

            switch (model.Type)
            {
                case IdentityTypes.USER:
                    profile = new UserProfile(model as UserRegistrationModel);
                    user = new User(model as UserRegistrationModel, profile);
                    break;
                case IdentityTypes.BIKE_MESSENGER:
                case IdentityTypes.CNG_DRIVER:
                    profile = new AssetProfile(model as AssetRegistrationModel);
                    user = new Asset(model as AssetRegistrationModel, profile as AssetProfile);
                    break;
                case IdentityTypes.ENTERPRISE:
                    var enterpriseProfile = new EnterpriseUserProfile(model as EnterpriseUserRegistrationModel);
                    user = new EnterpriseUser(model as EnterpriseUserRegistrationModel, enterpriseProfile);
                    break;

            }
            var identityResult = await accountManager.CreateAsync(user, model.Password);
            var creationResult = new AccountResult(identityResult, user);

            return creationResult;
        }

        public async Task<SendEmailResponse> NotifyUserCreationByMail(User user, string webcatUrl, string confirmEmailPath, string emailTemplatePath)
        {
            string code = await this.accountManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var confirmEmailRouteParams = new Dictionary<string, string>() { { "userId", user.Id }, { "code", code } };
            var confirmationUrl = string.Concat(webcatUrl, confirmEmailPath, confirmEmailRouteParams.ToQuerystring());           

            var result = await mailService.SendWelcomeMail(new SendWelcomeEmailRequest()
            {
                RecipientEmail = user.Email,
                RecipientUsername = user.UserName,
                ConfirmationUrl = confirmationUrl.ToString()
            }, emailTemplatePath);
            return result;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string code)
        {
            return await accountManager.ConfirmEmailAsync(userId, code);
        }

        public async Task<User> FindUser(string userName, string password)
        {
            return await accountManager.FindAsync(userName, password);
        }

        public async Task<User> FindUserByUserNameEmailPhoneNumber(string userKey, string password)
        {
            var user = await accountManager.FindByUserNameOrEmailOrPhoneNumber(userKey);
            return await FindUser(user.UserName, password);
        }

        public async Task<User> FindUserByEmail(string email, string password)
        {
            return await accountManager.FindByEmailAsync(email, password);
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingToken = await dbContext.RefreshTokens.Find(x => x.Subject == token.Subject && x.ClientId == token.ClientId).FirstOrDefaultAsync();

            if (existingToken != null)
            {
                // FIMXE: No check here on the result, not sure what to do here, if the remove refreshtoken fails, should we continue?
                var result = await RemoveRefreshToken(existingToken);
            }

            dbContext.RefreshTokens.InsertOne(token);
            return true;
        }

        public async Task<List<User>> FindAll(int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await accountManager.FindAll(page * pageSize, pageSize);
        }

        public async Task<IQueryable<UserModel>> FindAllAsModel()
        {
            return await accountManager.FindAllAsModel();
        }

        public async Task<List<UserModel>> FindAllAsModel(int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await accountManager.FindAllAsModel(page * pageSize, pageSize);
        }

        public async Task<PageEnvelope<UserModel>> FindAllEnvelopedAsModel(int page, int pageSize, HttpRequestMessage request, string routeName)
        {
            var data = await FindAllAsModel(page, pageSize);
            var total = await accountManager.GetTotalUserCount();

            return new PageEnvelope<UserModel>(total, page, pageSize, routeName, data, request);
        }

        public async Task<IdentityResult> Update(IdentityProfile profile, string userName)
        {
            var user = await accountManager.FindByNameAsync(userName);
            if (user.Type == IdentityTypes.USER && profile.GetType() != typeof(UserProfile))
                throw new InvalidOperationException("Updating User with different payload");
            else if (user.Type == IdentityTypes.ENTERPRISE && profile.GetType() != typeof(EnterpriseUserProfile))
                throw new InvalidOperationException("Updating Enterprise user with different payload");
            else if (user.Type != IdentityTypes.USER || user.Type != IdentityTypes.ENTERPRISE && profile.GetType() != typeof(AssetProfile))
                // INFO: Not changing pic url this way. :)
                // Would have a seperate update method, this shouldnt affect the PicUri
                profile.PicUri = user.Profile.PicUri;
            user.Profile = profile;

            var result = await accountManager.UpdateAsync(user);
            if (user.Type != IdentityTypes.USER && user.Type != IdentityTypes.ENTERPRISE)
            {
                var updateDef = Builders<Job>.Update.Set(x => x.Assets[user.Id], new AssetModel(user as Asset));
                var searchFilter = Builders<Job>.Filter.Exists(x => x.Assets[user.Id], true);
                var propagationResult = await dbContext.Jobs.UpdateManyAsync(searchFilter, updateDef);
            }
            else if (user.Roles.Any(x => x == "Administrator" || x == "BackOfficeAdmin"))
            {
                var userModel = new UserModel(user);
                var updateDef = Builders<Job>.Update.Set(x => x.JobServedBy, userModel);
                var searchFilter = Builders<Job>.Filter.Where(x => x.JobServedBy.UserId == user.Id);
                var propagationResult = await dbContext.Jobs.UpdateManyAsync(searchFilter, updateDef);
            }
            else if (user.Type == IdentityTypes.USER && user.Type == IdentityTypes.ENTERPRISE)
            {
                var userModel = user.Type == IdentityTypes.USER ? new UserModel(user) : new EnterpriseUserModel(user as EnterpriseUser);
                var updateDef = Builders<Job>.Update.Set(x => x.User, userModel);
                var searchFilter = Builders<Job>.Filter.Where(x => x.User.UserId == user.Id);
                var propagationResult = await dbContext.Jobs.UpdateManyAsync(searchFilter, updateDef);
            }

            //TODO: Need to do something with this propagation results man


            //FIXME: might have to do the same propagation for enterprise users
            return result;

        }

        public async Task<IdentityResult> UpdateById(IdentityProfile model, string userId)
        {
            var user = await accountManager.FindByIdAsync(userId);
            // INFO: Not changing pic url this way. :)
            // Would have a seperate update method, this shouldnt affect the PicUri
            model.PicUri = user.Profile.PicUri;
            user.Profile = model;

            return await accountManager.UpdateAsync(user);
        }

        public async Task<bool> IsUsernameAvailable(string suggestedUsername)
        {
            var user = await accountManager.FindByNameAsync(suggestedUsername);
            if (user == null)
                return true;
            return false;
        }

        public async Task<bool> IsPhoneNumberAvailable(string phoneNumber)
        {
            try
            {
                var user = await accountManager.FindByPhoneNumber(phoneNumber);
                if (user == null)
                    return true;
                return false;
            }
            catch (EntityNotFoundException)
            {
                return true;
            }
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            var user = await accountManager.FindByEmailAsync(email);
            if (user == null)
                return true;
            return false;
        }

        public async Task<IdentityResult> UpdateUsername(string newUserName, string oldUserName)
        {
            var user = await accountManager.FindByNameAsync(oldUserName);
            return await UpdateUsername(user, newUserName);
        }

        public async Task<IdentityResult> UpdateUsernameById(string userId, string newUserName)
        {
            var user = await accountManager.FindByIdAsync(userId);
            return await UpdateUsername(user, newUserName);
        }

        private async Task<IdentityResult> UpdateUsername(User user, string newUserName)
        {
            if (await IsUsernameAvailable(newUserName))
            {
                user.UserName = newUserName;
                var result = await accountManager.UpdateAsync(user).ConfigureAwait(continueOnCapturedContext: false);
                if (result.Succeeded && serviceBusClient?.AccountUpdateTopicClient != null)
                {
                    // INFO: This is definitely temporary, when GFETCH-250 finishes this would be done by another microservice.
                    var updateFilter = Builders<Job>.Update
                        .Set(x => x.User.UserName, newUserName)
                        .Set(x => x.ModifiedTime, DateTime.UtcNow);

                    var jobUpdateResult = await dbContext.Jobs.UpdateManyAsync(x => x.User.UserId == user.Id, updateFilter);
                    if (!jobUpdateResult.IsAcknowledged)
                    {
                        // TODO: Log it or do something about it as this means the propagation failed
                    }
                    return result;
                }
                else
                {
                    throw new ServerErrorException($"Failed to update username for {user.UserName} with id {user.Id}");
                }
            }
            else
            {
                return new IdentityResult(new string[] { "UserName " + newUserName + " is already taken" });
            }
        }

        // FIXME: I can fix this I think, the route to userName search wont be necessary if I can
        // provide user id right away from authcontext;
        public async Task<IdentityResult> UpdatePassword(PasswordUpdateModel model, string userName)
        {
            var user = await accountManager.FindByNameAsync(userName);
            return await accountManager.ChangePasswordAsync(user.Id, model.CurrentPassword, model.NewPassword);
        }

        public async Task<IdentityResult> UpdateContacts(ContactUpdateModel model, string userName)
        {
            var user = await accountManager.FindByNameAsync(userName);
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            return await accountManager.UpdateAsync(user);
        }

        public async Task<User> FindUser(string userId)
        {
            var user = await accountManager.FindByIdAsync(userId);
            if (user == null)
                throw new EntityNotFoundException("User", userId);
            return user;
        }

        public async Task<UserModel> FindUserAsModel(string userId)
        {
            var user = await FindUser(userId);

            // FIXME: Need to take care of this isAuthenticated Shit
            return user.ToModel(true);
        }

        public async Task<PageEnvelope<Job>> FindAssignedJobs(string userId, int page, int pageSize, DateTime? dateTimeUpto, JobState jobStateToFetchUpTo, SortDirection dateTimeSortDirection, HttpRequestMessage request, string apiRoute)
        {
            // INFO: When the job microservice and its classes gets to be generic, we should really refactor this one too
            // And we can use multithreading here to get the job done faster

            var FindContext = dateTimeUpto == null ?
                dbContext.Jobs.Find(x => x.Assets.ContainsKey(userId) && x.State == jobStateToFetchUpTo) :
                dbContext.Jobs.Find(x => x.Assets.ContainsKey(userId) && x.State == jobStateToFetchUpTo && x.CreateTime >= dateTimeUpto);
            var orderContext = dateTimeSortDirection == SortDirection.Descending ? FindContext.SortByDescending(x => x.CreateTime) : FindContext.SortBy(x => x.CreateTime);

            var data = new QueryResult<Job>()
            {
                Total = await orderContext.CountAsync(),
                Result = await orderContext.Skip(page * pageSize).Limit(pageSize).ToListAsync()
            };

            return new PageEnvelope<Job>(data.Total, page, pageSize, apiRoute, data.Result, request);
        }

        public async Task<PageEnvelope<Job>> FindAssignedJobsByUserName(string userName, int page, int pageSize, DateTime? dateTimeUpto, JobState jobStateToFetchUpTo, SortDirection dateTimeSortDirection, HttpRequestMessage request, string apiRoute)
        {
            var user = await accountManager.FindByNameAsync(userName);
            return await FindAssignedJobs(user.Id, page, pageSize, dateTimeUpto, jobStateToFetchUpTo, dateTimeSortDirection, request, apiRoute);
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            return await RemoveRefreshToken(refreshToken.Id);
        }

        public async Task<bool> RemoveRefreshToken(string hashedTokenId)
        {
            var result = await dbContext.RefreshTokens.DeleteOneAsync(x => x.Id == hashedTokenId);
            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await dbContext.RefreshTokens.Find(x => x.Id == refreshTokenId).FirstOrDefaultAsync();
            return refreshToken;
        }

        // FIXME: This is literally a crime, like literally, no freaking paging or anything
        // But Im too tired to do this, Why the hell I ever saw OData
        public async Task<List<RefreshToken>> GetAllRefreshTokens()
        {
            return await dbContext.RefreshTokens.Find(x => true).ToListAsync();
        }

        public async Task<FileUploadModel> UploadAvatar(HttpContent content, string userId)
        {
            var fileUploadModel = await blobService.UploadBlob(content, "avatar", StorageConstants.SupportedImageFormats);
            var result = await accountManager.ChangeAvatar(userId, fileUploadModel.FileUrl);
            if (result.ModifiedCount > 0)
                return fileUploadModel;
            else
            {
                // INFO: Upload was alright but our update failed, we should delete the file we
                // created on the blob storage or it will keep blocking stuff for us.

                if (!await blobService.TryDeleteBlob(fileUploadModel.FileName))
                {
                    // TODO: We should log that we failed to delete this
                }

                throw new UpdateFailedException("User.Profile.PicUrl");
            }
        }
    }
}