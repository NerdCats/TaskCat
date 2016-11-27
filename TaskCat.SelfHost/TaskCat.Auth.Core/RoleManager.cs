namespace TaskCat.Account.Core
{
    using Data.Entity.Identity;
    using Microsoft.AspNet.Identity;
    using AspNet.Identity.MongoDB;
    using Common.Db;

    public class RoleManager : RoleManager<Role>
    {
        private IDbContext dbContext;

        public RoleManager(IDbContext dbContext) : base(new RoleStore<Role>(dbContext.Roles))
        {
            this.dbContext = dbContext;
        }
    }
}