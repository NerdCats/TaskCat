namespace TaskCat.Lib.DropPoint
{
    using Db;
    using MongoDB.Driver;
    using Data.Entity;
    using Domain;
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using System.Linq;

    public class DropPointService : IDropPointService
    {
        private IMongoCollection<DropPoint> collection;

        public DropPointService(IDbContext dbContext)
        {
            this.collection = dbContext.DropPoints;
        }

        public async Task<DropPoint> Delete(string id)
        {
            var item = await Get(id);
            var result = await collection.DeleteOneAsync(x => x.Id == item.Id);
            if (result.DeletedCount > 0 && result.IsAcknowledged)
                return item;
            else
                throw new EntityDeleteException(typeof(DropPoint), item.Id);
        }

        public async Task<DropPoint> Get(string id)
        {
            var result = (await collection.Find(x => x.Id == id).ToListAsync()).FirstOrDefault();
            if (result == null)
            {
                throw new EntityNotFoundException(typeof(DropPoint), id);
            }
            return result;
        }

        public async Task<DropPoint> Insert(DropPoint obj)
        {
            await collection.InsertOneAsync(obj);
            return obj;
        }

        public async Task<DropPoint> Update(DropPoint obj)
        {
            if(obj.Id==null)
            {
                throw new ArgumentNullException(nameof(obj.Id));
            }
            var result = await collection.ReplaceOneAsync(x => x.Id == obj.Id, obj);
            if (result.IsAcknowledged)
            {
                if (result.MatchedCount == 0) throw new EntityNotFoundException(typeof(DropPoint), obj.Id);
                return obj;
            }

            throw new EntityUpdateException(typeof(DropPoint), obj.Id);
        }
    }
}