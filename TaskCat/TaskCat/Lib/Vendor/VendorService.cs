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
    using Exceptions;

    public class VendorService : IVendorService
    {
        private AccountManager accountManager;
        private IDbContext dbContext;

        public VendorService(AccountManager accountManager, IDbContext context)
        {
            this.dbContext = context;
            this.accountManager = accountManager;
            this.Collection = context.VendorProfiles;
        }

        public IMongoCollection<VendorProfile> Collection { get; }

        public async Task<VendorProfile> Delete(string id)
        {
            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id);

            var updateAccountFilter = Builders<User>.Filter.Where(x => x.Type == IdentityTypes.ENTERPRISE && (x as EnterpriseUser).VendorProfileId == id);
            var updateAccount = Builders<User>.Update.Set(nameof(EnterpriseUser.VendorProfileId), default(string));
            await dbContext.Users.UpdateOneAsync(updateAccountFilter, updateAccount);

            if (result == null)
                throw new EntityNotFoundException(typeof(VendorProfile), id);
            return result;
        }

        public async Task<VendorProfile> Get(string id)
        {
            var result = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (result == null)
                throw new EntityNotFoundException(typeof(VendorProfile), id);
            return result;
        }

        public async Task<VendorProfile> Insert(VendorProfile obj)
        {
            await Collection.InsertOneAsync(obj);
            return obj;
        }

        public async Task<SubscriptionResult> Subscribe(VendorProfile profile)
        {
            var user = (await accountManager.FindByIdAsync(profile.UserId) as EnterpriseUser);
            if (!string.IsNullOrWhiteSpace(user.VendorProfileId))
                throw new InvalidOperationException($"{user.Id} is already a vendor");

            if (user == null || user.Type != IdentityTypes.ENTERPRISE)
                throw new NotSupportedException($"User {profile.UserId} is not an enterprise user");

            if (string.IsNullOrWhiteSpace(user.VendorProfileId))
            {
                user.VendorSubscriptionDate = DateTime.UtcNow;
                var result = await accountManager.UpdateAsync(user);
                var insertionResult = await Insert(profile);
                user.VendorProfileId = profile.Id;
                if (result.Succeeded && insertionResult != null)
                    return SubscriptionResult.SUCCESS;
                else return SubscriptionResult.FAILED;
            }
            return SubscriptionResult.NOT_MODIFIED;
        }

        public async Task<VendorProfile> Update(VendorProfile profile)
        {
            var user = (await accountManager.FindByIdAsync(profile.UserId) as EnterpriseUser);
            if (string.IsNullOrWhiteSpace(user.VendorProfileId))
                throw new InvalidOperationException($"{user.Id} is not a vendor, please subscribe as a vendor first");

            var result = await Collection.FindOneAndReplaceAsync(x => x.UserId == profile.UserId && x.Id == profile.Id, profile);
            if (result == null)
                throw new EntityUpdateException(typeof(VendorProfile), profile.Id);
            return result;
        }
    }
}