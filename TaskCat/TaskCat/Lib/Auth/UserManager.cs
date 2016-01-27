namespace TaskCat.Lib.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.AspNet.Identity;
    using Data.Entity.Identity;
    using AspNet.Identity.MongoDB;
    using Db;

    public class UserManager : UserManager<User>
    {
        private IDbContext dbContext;

        public UserManager(IDbContext dbContext) : base(new UserStore<User>(dbContext.Users))
        {
            this.dbContext = dbContext;
        }
    }
}