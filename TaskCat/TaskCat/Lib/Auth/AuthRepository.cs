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

    public class AuthRepository
    {
        // FIXME: direct dbContext usage in repo.. Should I?
        private readonly IDbContext dbContext;
        private readonly UserManager<User> userManager;
        private readonly UserManager<Asset> assetManager;
        

        public AuthRepository(IDbContext dbContext, UserManager<User> userManager, UserManager<Asset> assetManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.assetManager = assetManager;
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
                    return await userManager.CreateAsync(user, model.Password);
                default:
                    profile = new AssetProfile(model as AssetRegistrationModel);
                    user = new Asset(model as AssetRegistrationModel, profile as AssetProfile);
                    return await assetManager.CreateAsync(user as Asset, model.Password);
            }
           
         
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
            User user = await userManager.FindAsync(userName, password);
            return user;
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

        internal async Task<User> FindUser(string userId)
        {
            return await userManager.FindByIdAsync(userId);
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