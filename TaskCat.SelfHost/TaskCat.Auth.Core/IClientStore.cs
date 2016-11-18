using System.Collections.Generic;
using System.Threading.Tasks;
using TaskCat.Data.Entity.Identity;
using TaskCat.Data.Model;

namespace TaskCat.Account.Core
{
    /// <summary>
    /// Generic interface to define a database store for clients
    /// </summary>
    public interface IClientStore
    {
        /// <summary>
        /// Activate a client by id.
        /// </summary>
        /// <param name="id">Client id.</param>
        /// <returns></returns>
        Task<Client> Activate(string id);

        /// <summary>
        /// Add a client to the database.
        /// </summary>
        /// <param name="model">Client model to create a client entry.</param>
        /// <returns>The new client that got created.</returns>
        Task<Client> AddClient(ClientModel model);

        /// <summary>
        /// Delete a client by id.
        /// </summary>
        /// <param name="id">Id for the client.</param>
        /// <returns>true if the client is deleted and false otherwise.</returns>
        Task<bool> DeleteClient(string id);

        /// <summary>
        /// Find a client by id.
        /// </summary>
        /// <param name="id">Id to serch the client against.</param>
        /// <returns>Client with the searched id.</returns>
        Task<Client> FindClient(string id);

        /// <summary>
        /// Gets the existing client count from database.
        /// </summary>
        /// <returns>Number of clients in the database.</returns>
        Task<long> GetClientsCount();

        /// <summary>
        /// Get all the clients/audiences listed in the database
        /// </summary>
        /// <returns>The IEnumerable of clients listed in the database</returns>
        Task<IEnumerable<Client>> GetAllClients();
    }
}