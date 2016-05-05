namespace TaskCat.Lib.Auth
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
    using Data.Model;
    using Utility;

    public class AccountRepository
    {
        // FIXME: direct dbContext usage in repo.. Should I?
        private readonly IDbContext dbContext;
        private readonly AccountManager accountManager;
        private readonly IBlobService blobService;
        private readonly JobManager jobManager;

        public AccountRepository(
            IDbContext dbContext,
            AccountManager accoutnManager,
            IBlobService blobService,
            JobManager jobManager)
        {
            this.dbContext = dbContext;
            this.accountManager = accoutnManager;
            this.blobService = blobService;
            this.jobManager = jobManager;
        }

        // Register is always used for someone not in the database, only first time User or first time Asset use this method
        internal async Task<IdentityResult> RegisterUser(RegistrationModelBase model)
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

        internal async Task<User> FindUserByUserNameEmailPhoneNumber(string userKey, string password)
        {
            var user = await accountManager.FindByUserNameOrEmailOrPhoneNumber(userKey);
            return await FindUser(user.UserName, password);
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

        internal async Task<List<UserModel>> FindAllAsModel(int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await accountManager.FindAllAsModel(page * pageSize, pageSize);
        }

        internal async Task<PageEnvelope<UserModel>> FindAllEnvelopedAsModel(int page, int pageSize, HttpRequestMessage request)
        {
            var data = await FindAllAsModel(page, pageSize);
            var total = await accountManager.GetTotalUserCount();

            return new PageEnvelope<UserModel>(total, page, pageSize, AppConstants.DefaultApiRoute, data, request);
        }

        internal async Task<IdentityResult> Update(IdentityProfile profile, string userName)
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
            else if(user.Roles.Any(x=>x=="Administrator" || x== "BackOfficeAdmin"))
            {
                var userModel = new UserModel(user);
                var updateDef = Builders<Job>.Update.Set(x => x.JobServedBy, userModel);
                var searchFilter = Builders<Job>.Filter.Where(x => x.JobServedBy.UserId == user.Id);
                var propagationResult = await dbContext.Jobs.UpdateManyAsync(searchFilter, updateDef);
            }
            else if(user.Type== IdentityTypes.USER && user.Type == IdentityTypes.ENTERPRISE)
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

        internal async Task<IdentityResult> UpdateById(IdentityProfile model, string userId)
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

        internal async Task<IdentityResult> UpdateUsername(string newUserName, string oldUserName)
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
            var user =  await accountManager.FindByIdAsync(userId);
            if (user == null)
                throw new EntityNotFoundException("User", userId);
            return user;
        }

        internal async Task<UserModel> FindUserAsModel(string userId)
        {
            var user = await FindUser(userId);

            // FIXME: Need to take care of this isAuthenticated Shit
            return user.ToModel(true);
        }

        internal async Task<PageEnvelope<Job>> FindAssignedJobs(string userId, int page, int pageSize, DateTime? dateTimeUpto, JobState jobStateToFetchUpTo, SortDirection dateTimeSortDirection, HttpRequestMessage request)
        {
            QueryResult<Job> data = await jobManager.GetJobsAssignedToUser(userId, page * pageSize, pageSize, dateTimeUpto, jobStateToFetchUpTo, dateTimeSortDirection);
            return new PageEnvelope<Job>(data.Total, page, pageSize, AppConstants.DefaultApiRoute, data.Result, request, request.GetQueryNameValuePairs().ToDictionary(x => x.Key, y => y.Value));
        }

        internal async Task<PageEnvelope<Job>> FindAssignedJobsByUserName(string userName, int page, int pageSize, DateTime? dateTimeUpto, JobState jobStateToFetchUpTo, SortDirection dateTimeSortDirection, HttpRequestMessage request)
        {
            var user = await accountManager.FindByNameAsync(userName);
            return await FindAssignedJobs(user.Id, page, pageSize, dateTimeUpto, jobStateToFetchUpTo, dateTimeSortDirection, request);
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
            var fileUploadModel = await blobService.UploadBlob(content, "avatar", AppConstants.SupportedImageFormats);
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