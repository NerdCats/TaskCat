namespace TaskCat.Auth.Core
{
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using Data.Entity.Identity;
    using System;
    using System.Security.Cryptography;
    using Data.Model.Identity;
    using Data.Model;
    using Common.Db;
    using Microsoft.Owin.Security.DataHandler.Encoder;

    /// <summary>
    /// Default implementation of IClientStore
    /// </summary>
    public class ClientStore : IClientStore
    {
        private readonly IDbContext dbContext;

        /// <summary>
        /// Creates an instance of ClientStore.
        /// </summary>
        /// <param name="dbContext">Database context to create the store with.</param>
        public ClientStore(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Find a client by id.
        /// </summary>
        /// <param name="id">Id to serch the client against.</param>
        /// <returns>Client with the searched id.</returns>
        public async Task<Client> FindClient(string id)
        {
            var client = await dbContext.Clients.Find(x => x.Id == id).FirstOrDefaultAsync();
            return client;
        }

        /// <summary>
        /// Add a client to the database.
        /// </summary>
        /// <param name="model">Client model to create a client entry.</param>
        /// <returns>The new client that got created.</returns>
        public async Task<Client> AddClient(ClientModel model)
        {
            var key = new byte[32];
            RNGCryptoServiceProvider.Create().GetBytes(key);
            var base64Secret = TextEncodings.Base64Url.Encode(key);

            Client newClient = new Client
            {
                Id = model.Id,
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

        /// <summary>
        /// Delete a client by id.
        /// </summary>
        /// <param name="id">Id for the client.</param>
        /// <returns>true if the client is deleted and false otherwise.</returns>
        public async Task<bool> DeleteClient(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var result = await dbContext.Clients.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged;
        }

        /// <summary>
        /// Find a client by id.
        /// </summary>
        /// <param name="id">Id to serch the client against.</param>
        /// <returns>Client with the searched id.</returns>
        public async Task<Client> Activate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            var filter = Builders<Client>.Update.Set(x => x.Active, true);
            var result = await dbContext.Clients.FindOneAndUpdateAsync(x=>x.Id == id, filter);
            return result;
        }

        /// <summary>
        /// Gets the existing client count from database.
        /// </summary>
        /// <returns>Number of clients in the database.</returns>
        public async Task<long> GetClientsCount()
        {
            var result = await dbContext.Clients.CountAsync(Builders<Client>.Filter.Empty);
            return result;
        }
    }
}
