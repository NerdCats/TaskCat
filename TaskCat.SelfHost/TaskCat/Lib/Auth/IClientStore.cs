using System.Threading.Tasks;
using TaskCat.Data.Entity.Identity;
using TaskCat.Data.Model;

namespace TaskCat.Lib.Auth
{
    public interface IClientStore
    {
        Task<Client> Activate(string id);
        Task<Client> AddClient(ClientModel model);
        Task<bool> DeleteClient(string id);
        Task<Client> FindClient(string clientId);
    }
}