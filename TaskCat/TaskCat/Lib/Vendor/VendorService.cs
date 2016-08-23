namespace TaskCat.Lib.Vendor
{
    using Auth;
    using Data.Entity.Identity;
    using Data.Model.Identity;
    using System;
    using System.Threading.Tasks;

    public class VendorService : IVendorService
    {
        private AccountManager accountManager;

        public VendorService(AccountManager accountManager)
        {
            this.accountManager = accountManager;
        }

        public async Task<SubscriptionResult> Subscribe(string userId)
        {
            var user = (await accountManager.FindByIdAsync(userId) as EnterpriseUser);




            if (user == null || user.Type!= IdentityTypes.ENTERPRISE)
                throw new NotSupportedException($"User {userId} is not an enterprise user");

            if (!user.IsVendor)
            {
                user.IsVendor = true;
                user.VendorSubscriptionDate = DateTime.UtcNow;
                var result = await accountManager.UpdateAsync(user);
                if (result.Succeeded)
                    return SubscriptionResult.SUCCESS;
                else return SubscriptionResult.FAILED;

            }
            return SubscriptionResult.NOT_MODIFIED; 
        }
    }
}