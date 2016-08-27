namespace TaskCat.Lib.Vendor
{
    using Auth;
    using Data.Entity.Identity;
    using Data.Model.Identity;
    using System;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Data.Entity;
    using Db;

    public class VendorService : IVendorService
    {
        private AccountManager accountManager;

        public VendorService(AccountManager accountManager, IDbContext context)
        {
            this.accountManager = accountManager;
            this.Collection = context.VendorProfiles;
        }

        public IMongoCollection<VendorProfile> Collection { get; }

        public Task<VendorProfile> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<VendorProfile> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<VendorProfile> Insert(VendorProfile obj)
        {
            await Collection.InsertOneAsync(obj);
            return obj;
        }

        public async Task<SubscriptionResult> Subscribe(string userId, VendorProfile profile)
        {
            var user = (await accountManager.FindByIdAsync(userId) as EnterpriseUser);

            if (user == null || user.Type!= IdentityTypes.ENTERPRISE)
                throw new NotSupportedException($"User {userId} is not an enterprise user");

            if (!user.IsVendor)
            {
                user.IsVendor = true;
                user.VendorSubscriptionDate = DateTime.UtcNow;
                var result = await accountManager.UpdateAsync(user);
                var insertionResult = await Insert(profile);
                if (result.Succeeded && insertionResult != null)
                    return SubscriptionResult.SUCCESS;
                else return SubscriptionResult.FAILED;
            }
            return SubscriptionResult.NOT_MODIFIED; 
        }

        public Task<VendorProfile> Update(VendorProfile obj)
        {
            throw new NotImplementedException();
        }
    }
}