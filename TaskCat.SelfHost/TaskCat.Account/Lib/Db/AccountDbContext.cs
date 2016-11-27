using AspNet.Identity.MongoDB;
using TaskCat.Common.Db;

namespace TaskCat.Account.Lib.Db
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext()
        {

        }
        private void EnsureIndexes()
        {
            IndexChecks.EnsureUniqueIndexOnUserName(this.Users);
            IndexChecks.EnsureUniqueIndexOnEmail(this.Users);
            IndexChecks.EnsureUniqueIndexOnRoleName(this.Roles);
            IndexFacade.EnsureUniqueIndexOnPhoneNumber(this.Users);
            IndexFacade.EnsureIndexesOnUserType(this.Users);
        }
    }
}
