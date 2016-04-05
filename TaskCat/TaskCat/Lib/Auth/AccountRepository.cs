﻿namespace TaskCat.Lib.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Entity.Identity;
    using Db;
    using Microsoft.AspNet.Identity;
    using Data.Model.Identity;
    using MongoDB.Driver;
    using Data.Entity;
    using Data.Model.Identity.Registration;
    using Data.Model.Identity.Profile;
    using System.Net.Http;
    using Model.Pagination;
    using Constants;
    using Data.Model.Identity.Response;
    using Storage;
    using Model.Storage;
    using Exceptions;
    using Job;
    using Data.Model.Query;

    public class AccountRepository
    {
        // FIXME: direct dbContext usage in repo.. Should I?
        private readonly IDbContext dbContext;
        private readonly AccountManger accountManager;
        private readonly IBlobService blobService;
        private readonly JobManager jobManager;

        public AccountRepository(
            IDbContext dbContext, 
            AccountManger accoutnManager, 
            IBlobService blobService,
            JobManager jobManager)
        {
            this.dbContext = dbContext;
            this.accountManager = accoutnManager;
            this.blobService = blobService;
            this.jobManager = jobManager;
        }

        // Register is always used for someone not in the database, only first time User or first time Asset use this method
        internal async Task<IdentityResult> RegisterUser(UserRegistrationModel model)
        {
            UserProfile profile;
            User user;

            switch (model.Type)
            {
                case IdentityTypes.USER:
                    profile = new UserProfile(model);
                    user = new User(model, profile);
                    break;
                default:
                    profile = new AssetProfile(model as AssetRegistrationModel);
                    user = new Asset(model as AssetRegistrationModel, profile as AssetProfile);
                    break;
            }

            return await accountManager.CreateAsync(user, model.Password);
        }


        internal async Task<Client> FindClient(string clientId)
        {
            // FIXME: Im not sure whether we'd need a client manager or not, if there's no controller for it
            // I dont see a reason though
            var client = await dbContext.Clients.Find(x => x.Id == clientId).FirstOrDefaultAsync();
            return client;
        }

        internal async Task<User> FindUser(string userName, string password)
        {
            return await accountManager.FindAsync(userName, password);
        }

        internal async Task<User> FindUserByEmail(string email, string password)
        {
            return await accountManager.FindByEmailAsync(email, password);
        }

        internal async Task<bool> AddRefreshToken(RefreshToken token)
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

        internal async Task<List<User>> FindAll(int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await accountManager.FindAll(page * pageSize, pageSize);
        }

        internal async Task<IQueryable<UserModel>> FindAllAsModel()
        {
            return await accountManager.FindAllAsModel();
        }

        internal async Task<IQueryable<UserModel>> FindAllAsModelAsQueryable(int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await accountManager.FindAllAsModelAsQueryable(page * pageSize, pageSize);
        }

        internal async Task<List<UserModel>> FindAllAsModel(int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await accountManager.FindAllAsModel(page * pageSize, pageSize);
        }

        internal async Task<PageEnvelope<User>> FindAllEnveloped(int page, int pageSize, HttpRequestMessage request)
        {
            var data = await FindAll(page, pageSize);
            var total = await accountManager.GetTotalUserCount();

            return new PageEnvelope<User>(total, page, pageSize, AppConstants.DefaultApiRoute, data, request);
        }

        internal async Task<PageEnvelope<UserModel>> FindAllEnvelopedAsModel(int page, int pageSize, HttpRequestMessage request)
        {
            var data = await FindAllAsModel(page, pageSize);
            var total = await accountManager.GetTotalUserCount();

            return new PageEnvelope<UserModel>(total, page, pageSize, AppConstants.DefaultApiRoute, data, request);
        }

        internal async Task<IdentityResult> Update(UserProfile model, string userName)
        {
            var user = await accountManager.FindByNameAsync(userName);
            if (user.Type != IdentityTypes.USER && model.GetType() != typeof(AssetProfile))
                throw new InvalidOperationException("Updating Asset with UserProfile payload");
            // INFO: Not changing pic url this way. :)
            // Would have a seperate update method, this shouldnt affect the PicUri
            model.PicUri = user.Profile.PicUri;
            user.Profile = model;

            var result = await accountManager.UpdateAsync(user);
            if (user.Type != IdentityTypes.USER)
            {
                var updateDef = Builders<Job>.Update.Set(x => x.Assets[user.Id], new AssetModel(user as Asset));
                var searchFilter = Builders<Job>.Filter.Exists(x => x.Assets[user.Id], true);
                var propagationResult = await dbContext.Jobs.UpdateManyAsync(searchFilter, updateDef);                    
            }
            return result;

        }

        internal async Task<IdentityResult> UpdateById(UserProfile model, string userId)
        {
            var user = await accountManager.FindByIdAsync(userId);
            // INFO: Not changing pic url this way. :)
            // Would have a seperate update method, this shouldnt affect the PicUri
            model.PicUri = user.Profile.PicUri;
            user.Profile = model;

            return await accountManager.UpdateAsync(user);
        }

        internal async Task<bool> IsUsernameAvailable(string suggestedUsername)
        {
            try
            {
                var user = await accountManager.FindByNameAsync(suggestedUsername);
                if (user == null)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal async Task<IdentityResult> UpdateUsername(string newUserName, string oldUserName )
        {
            var user = await accountManager.FindByNameAsync(oldUserName);
            if (await IsUsernameAvailable(newUserName))
            {
                user.UserName = newUserName;
                return await accountManager.UpdateAsync(user);
            }
            else
            {
                return new IdentityResult(new string[] { "UserName " + newUserName + " is already taken" });
            }
        }

        internal async Task<IdentityResult> UpdateUsernameById(string userId, string newUserName)
        {
            var user = await accountManager.FindByIdAsync(userId);
            if (await IsUsernameAvailable(newUserName))
            {
                user.UserName = newUserName;
                return await accountManager.UpdateAsync(user);
            }
            else
            {
                return new IdentityResult(new string[] { "UserName " + newUserName + " is already taken" });
            }
        }

       



        // FIXME: I can fix this I think, the route to userName search wont be necessary if I can
        // provide user id right away from authcontext;
        internal async Task<IdentityResult> UpdatePassword(PasswordUpdateModel model, string userName)
        {
            var user = await accountManager.FindByNameAsync(userName);
            return await accountManager.ChangePasswordAsync(user.Id, model.CurrentPassword, model.NewPassword);
        }

        internal async Task<IdentityResult> UpdateContacts(ContactUpdateModel model, string userName)
        {
            var user = await accountManager.FindByNameAsync(userName);
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            return await accountManager.UpdateAsync(user);
        }

        internal async Task<User> FindUser(string userId)
        {
            return await accountManager.FindByIdAsync(userId);
        }

        internal async Task<UserModel> FindUserAsModel(string userId)
        {
            var user = await FindUser(userId);

            //FIXME: Someday I can use a factory here
            if (user.Type == IdentityTypes.USER)
                return new UserModel(user);
            else return new AssetModel(user as Asset);
        }

        internal async Task<PageEnvelope<Job>> FindAssignedJobs(string userId, int page, int pageSize, HttpRequestMessage request)
        {
            QueryResult<Job> data = await jobManager.GetJobsAssignedToUser(userId, page, pageSize);
            return new PageEnvelope<Job>(data.Total, page, pageSize, AppConstants.DefaultApiRoute, data.Result, request, request.GetQueryNameValuePairs().ToDictionary(x => x.Key, y => y.Value));
        }

        internal async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            return await RemoveRefreshToken(refreshToken.Id);
        }

        internal async Task<bool> RemoveRefreshToken(string hashedTokenId)
        {
            var result = await dbContext.RefreshTokens.DeleteOneAsync(x => x.Id == hashedTokenId);
            return result.IsAcknowledged && result.DeletedCount == 1;
        }

        internal async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await dbContext.RefreshTokens.Find(x => x.Id == refreshTokenId).FirstOrDefaultAsync();
            return refreshToken;
        }

        

        // FIXME: This is literally a crime, like literally, no freaking paging or anything
        // But Im too tired to do this, Why the hell I ever saw OData
        internal async Task<List<RefreshToken>> GetAllRefreshTokens()
        {
            return await dbContext.RefreshTokens.Find(x => true).ToListAsync();
        }

        internal async Task<FileUploadModel> UploadAvatar(HttpContent content, string userId)
        {
            var fileUploadModel =  await blobService.UploadBlob(content, "avatar", AppConstants.SupportedImageFormats);
            var result = await accountManager.ChangeAvatar(userId, fileUploadModel.FileUrl);
            if(result.ModifiedCount>0)
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