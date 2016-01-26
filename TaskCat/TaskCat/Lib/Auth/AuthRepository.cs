namespace TaskCat.Lib.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity.Identity;

    public class AuthRepository
    {
        internal Client FindClient(string clientId)
        {
            throw new NotImplementedException();
        }

        internal async Task<User> FindUser(string userName, string password)
        {
            throw new NotImplementedException();
        }

        internal async Task<bool> AddRefreshToken(RefreshToken token)
        {
            throw new NotImplementedException();
        }

        internal async Task<RefreshToken> FindRefreshToken(string hashedTokenId)
        {
            throw new NotImplementedException();
        }

        internal async Task<bool> RemoveRefreshToken(string hashedTokenId)
        {
            throw new NotImplementedException();
        }
    }
}