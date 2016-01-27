namespace TaskCat.Lib.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.AspNet.Identity;
    using Data.Entity.Identity;

    public class UserManager : UserManager<User>
    {
        private IUserStore<User> _store; 
        public UserManager(IUserStore<User> store) : base(store)
        {
            this._store = store;
        }

        internal Task<IdentityResult> CreateAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        internal Task<User> FindAsync(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}