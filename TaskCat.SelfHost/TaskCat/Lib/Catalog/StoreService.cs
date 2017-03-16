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

    public class StoreService : IRepository<DataTag>
    {
        public IMongoCollection<DataTag> Collection { get; set; }

        public StoreService(IDbContext dbContext)
        {
            this.Collection = dbContext.Stores;
        }

        public async Task<DataTag> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id);

            if (result == null)
                throw new EntityDeleteException(typeof(DataTag), id);
            return result;
        }

        public async Task<DataTag> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = (await Collection.Find(x => x.Id == id).ToListAsync()).FirstOrDefault();
            if (result == null)
            {
                throw new EntityNotFoundException(typeof(DataTag), id);
            }
            return result;
        }

        public async Task<DataTag> Insert(DataTag store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            
            await Collection.InsertOneAsync(store);
            return store;
        }

        public async Task<DataTag> Update(DataTag store)
        {
            if (string.IsNullOrWhiteSpace(store.Id)) throw new ArgumentNullException(nameof(store.Id));
            if (String.IsNullOrWhiteSpace(store.EnterpriseUserId)) throw new ArgumentNullException(nameof(store.EnterpriseUserId));
            
            var result = await Collection.FindOneAndReplaceAsync(x => x.Id == store.Id && x.EnterpriseUserId == store.EnterpriseUserId, store);
            if (result == null)
                throw new EntityUpdateException(typeof(DataTag), store.Id);
            return result;
        }
    }
}