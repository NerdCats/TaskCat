namespace TaskCat.Lib.Store
{
    using System;
    using System.Threading.Tasks;
    using Data.Entity;
    using MongoDB.Driver;
    using Db;
    using Exceptions;
    using System.Linq;

    public class StoreService : IStoreService
    {
        public IMongoCollection<Store> Collection { get; set; }

        public StoreService(IDbContext dbContext)
        {
            this.Collection = dbContext.Stores;
        }

        public async Task<Store> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id);
            if (result == null)
                throw new EntityDeleteException(typeof(Store), result.Id);
            return result;
        }

        public async Task<Store> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
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
            await Collection.InsertOneAsync(store);
            return store;
        }

        public Task<Store> Update(Store obj)
        {
            throw new NotImplementedException();
        }
    }
}