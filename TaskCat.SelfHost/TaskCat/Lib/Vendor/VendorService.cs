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
    using Common.Exceptions;
    using Common.Db;
    using Auth.Core;

    public class VendorService : IVendorService
    {
        private AccountManager accountManager;
        private IDbContext dbContext;

        public VendorService(AccountManager accountManager, IDbContext context)
        {
            this.dbContext = context;
            this.accountManager = accountManager;
            this.Collection = context.Vendors;
        }

        public IMongoCollection<Vendor> Collection { get; }

        public async Task<Vendor> Delete(string id)
        {
            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id);

            var updateAccountFilter = Builders<User>.Filter.Where(x => x.Type == IdentityTypes.ENTERPRISE && (x as EnterpriseUser).VendorId == id);
            var updateAccount = Builders<User>.Update.Set(nameof(EnterpriseUser.VendorId), default(string));
            await dbContext.Users.UpdateOneAsync(updateAccountFilter, updateAccount);

            if (result == null)
                throw new EntityNotFoundException(typeof(Vendor), id);
            return result;
        }

        public async Task<Vendor> Get(string id)
        {
            var result = await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (result == null)
                throw new EntityNotFoundException(typeof(Vendor), id);
            return result;
        }

        public async Task<Vendor> Insert(Vendor obj)
        {
            await Collection.InsertOneAsync(obj);
            return obj;
        }

        public async Task<SubscriptionResult> Subscribe(Vendor vendor)
        {
            var user = (await accountManager.FindByIdAsync(vendor.UserId) as EnterpriseUser);
            if (!string.IsNullOrWhiteSpace(user.VendorId))
                throw new InvalidOperationException($"{user.Id} is already a vendor");

            if (user == null || user.Type != IdentityTypes.ENTERPRISE)
                throw new NotSupportedException($"User {vendor.UserId} is not an enterprise user");

            if (string.IsNullOrWhiteSpace(user.VendorId))
            {
                user.VendorSubscriptionDate = DateTime.UtcNow;
                var result = await accountManager.UpdateAsync(user);
                var insertionResult = await Insert(vendor);
                user.VendorId = vendor.Id;
                if (result.Succeeded && insertionResult != null)
                    return SubscriptionResult.SUCCESS;
                else return SubscriptionResult.FAILED;
            }
            return SubscriptionResult.NOT_MODIFIED;
        }

        public async Task<Vendor> Update(Vendor profile)
        {
            var user = (await accountManager.FindByIdAsync(profile.UserId) as EnterpriseUser);
            if (string.IsNullOrWhiteSpace(user.VendorId))
                throw new InvalidOperationException($"{user.Id} is not a vendor, please subscribe as a vendor first");

            var result = await Collection.FindOneAndReplaceAsync(x => x.UserId == profile.UserId && x.Id == profile.Id, profile);
            if (result == null)
                throw new EntityUpdateException(typeof(Vendor), profile.Id);
            return result;
        }
    }
}