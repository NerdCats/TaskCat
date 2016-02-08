namespace TaskCat.Lib.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity.Identity;
    using Db;
    using Microsoft.AspNet.Identity;
    using Data.Model.Identity;
    using MongoDB.Driver;
    using Data.Entity;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using Asset;
    using System.Web.Http;
    using Data.Model.Identity.Registration;
    using Data.Model.Identity.Profile;
    using System.Net.Http;
    using TaskCat.Model.Pagination;
    using Constants;

    public class AuthRepository
    {
        // FIXME: direct dbContext usage in repo.. Should I?
        private readonly IDbContext dbContext;
        private readonly AccountManger accountManager;


        public AuthRepository(IDbContext dbContext, AccountManger accoutnManager)
        {
            this.dbContext = dbContext;
            this.accountManager = accoutnManager;
        }

        // Register is always used for someone not in the database, only first time User or first time Asset use this method
        public async Task<IdentityResult> RegisterUser(UserRegistrationModel model)
        {
            UserProfile profile;
            User user;

            switch (model.Type)
            {
                case IdentityTypes.FETCHER:
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

        internal async Task<T> FindUser<T>(string userName, string password) where T : User
        {
            return await accountManager.FindAsByAsync<T>(userName, password);
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

        internal async Task<PageEnvelope<User>> FindAllEnveloped(int page, int pageSize, HttpRequestMessage request)
        {
            var data = await FindAll(page, pageSize);
            var total = await accountManager.GetTotalUserCount();

            return new PageEnvelope<User>(total, page, pageSize, AppConstants.DefaultApiRoute, data, request);
        }

        internal async Task<User> FindUser(string userId)
        {
            return await accountManager.FindByIdAsync(userId);
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
        public async Task<List<RefreshToken>> GetAllRefreshTokens()
        {
            return await dbContext.RefreshTokens.Find(x => true).ToListAsync();
        }
    }
}