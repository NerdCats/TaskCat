namespace TaskCat.Lib.Auth
{
    using Db;
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using Data.Entity.Identity;
    using System;
    using System.Security.Cryptography;
    using Microsoft.Owin.Security.DataHandler.Encoder;
    using Data.Model.Identity;
    using Data.Model;

    public class ClientStore : IClientStore
    {
        private readonly IDbContext dbContext;

        public ClientStore(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Client> FindClient(string clientId)
        {
            var client = await dbContext.Clients.Find(x => x.Id == clientId).FirstOrDefaultAsync();
            return client;
        }

        public async Task<Client> AddClient(ClientModel model)
        {
            var clientId = Guid.NewGuid().ToString("N");

            var key = new byte[32];
            RNGCryptoServiceProvider.Create().GetBytes(key);
            var base64Secret = TextEncodings.Base64Url.Encode(key);

            Client newClient = new Client
            {
                Id = clientId,
                Secret = base64Secret,
                Active = model.Active,
                Name = model.Name,
                ApplicationType = ApplicationTypes.NativeConfidential,
                AllowedOrigin = model.AllowedOrigin,
                RefreshTokenLifeTime = model.RefreshTokenLifeTime
            };

            await dbContext.Clients.InsertOneAsync(newClient);
            return newClient;
        }

        public async Task<bool> DeleteClient(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var result = await dbContext.Clients.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged;
        }

        public async Task<Client> Activate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var filter = Builders<Client>.Update.Set(x => x.Active, true);
            var result = await dbContext.Clients.FindOneAndUpdateAsync(x=>x.Id == id, filter);
            return result;
        }
    }
}
