namespace TaskCat.Lib.Catalog
{
    using System;
    using System.Threading.Tasks;
    using Data.Entity;
    using MongoDB.Driver;
    using System.Linq;
    using Common.Domain;
    using Common.Exceptions;
    using Common.Db;

    public class StoreService : IRepository<Store>
    {
        public IMongoCollection<Store> Collection { get; set; }

        public StoreService(IDbContext dbContext)
        {
            this.Collection = dbContext.Stores;
        }

        public async Task<Store> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id);

            if (result == null)
                throw new EntityDeleteException(typeof(Store), id);
            return result;
        }

        public async Task<Store> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = (await Collection.Find(x => x.Id == id).ToListAsync()).FirstOrDefault();
            if (result == null)
            {
                throw new EntityNotFoundException(typeof(Store), id);
            }
            return result;
        }

        public async Task<Store> Insert(Store store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            
            await Collection.InsertOneAsync(store);
            return store;
        }

        public async Task<Store> Update(Store store)
        {
            if (string.IsNullOrWhiteSpace(store.Id)) throw new ArgumentNullException(nameof(store.Id));
            if (String.IsNullOrWhiteSpace(store.EnterpriseUserId)) throw new ArgumentNullException(nameof(store.EnterpriseUserId));
            
            var result = await Collection.FindOneAndReplaceAsync(x => x.Id == store.Id && x.EnterpriseUserId == store.EnterpriseUserId, store);
            if (result == null)
                throw new EntityUpdateException(typeof(Store), store.Id);
            return result;
        }
    }
}