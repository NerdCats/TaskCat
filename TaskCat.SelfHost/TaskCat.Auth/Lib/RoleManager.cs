﻿namespace TaskCat.Auth.Lib
{
    using Data.Entity.Identity;
    using Microsoft.AspNet.Identity;
    using Db;
    using AspNet.Identity.MongoDB;

    public class RoleManager : RoleManager<Role>
    {
        private IDbContext dbContext;

        public RoleManager(IDbContext dbContext) : base(new RoleStore<Role>(dbContext.Roles))
        {
            this.dbContext = dbContext;
        }
    }
}