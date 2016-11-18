using AspNet.Identity.MongoDB;
using TaskCat.Common.Db;

namespace TaskCat.Auth.Lib.Db
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext()
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
