namespace TaskCat.Account.Core.Model
{
    using Microsoft.AspNet.Identity;
    using Data.Entity.Identity;

    public class AccountResult
    {
        public User User { get; set; }
        public IdentityResult Result { get; set; }

        public AccountResult(IdentityResult result, User user)
        {
            this.Result = result;
            this.User = user;
        }
    }
}