namespace TaskCat.Lib.DropPoint
{
    using Db;
    using MongoDB.Driver;
    using Data.Entity;
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using System.Linq;
    using System.Collections.Generic;
    using MongoDB.Bson;
    public class DropPointService : IDropPointService
    {
        public IMongoCollection<DropPoint> Collection { get; set; }

        public DropPointService(IDbContext dbContext)
        {
            this.Collection = dbContext.DropPoints;
        }

        public async Task<DropPoint> Delete(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var item = await Get(id);
            var result = await Collection.DeleteOneAsync(x => x.Id == item.Id);
            if (result.DeletedCount > 0 && result.IsAcknowledged)
                return item;
            else
                throw new EntityDeleteException(typeof(DropPoint), item.Id);
        }

        public async Task<DropPoint> Get(string id)
        {
            var result = (await Collection.Find(x => x.Id == id).ToListAsync()).FirstOrDefault();
            if (result == null)
            {
                throw new EntityNotFoundException(typeof(DropPoint), id);
            }
            return result;
        }

        public async Task<DropPoint> Insert(DropPoint obj)
        {
            await Collection.InsertOneAsync(obj);
            return obj;
        }

        public async Task<DropPoint> Update(DropPoint obj)
        {
            if (obj.Id == null)
            {
                throw new ArgumentNullException(nameof(obj.Id));
            }
            var result = await Collection.ReplaceOneAsync(x => x.Id == obj.Id, obj);
            return UpdateResult(obj, result);
        }
        public async Task<DropPoint> Update(DropPoint obj, string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (obj.Id == null)
            {
                throw new ArgumentNullException(nameof(obj.Id));
            }

            var result = await Collection.ReplaceOneAsync(x => x.Id == obj.Id && x.UserId == userId, obj);
            return UpdateResult(obj, result);
        }

        private static DropPoint UpdateResult(DropPoint obj, ReplaceOneResult result)
        {
            if (result.IsAcknowledged)
            {
                if (result.MatchedCount == 0) throw new EntityNotFoundException(typeof(DropPoint), obj.Id);
                return obj;
            }

            throw new EntityUpdateException(typeof(DropPoint), obj.Id);
        }

        

        public async Task<IEnumerable<DropPoint>> SearchDropPoints(string userId, string query)
        {
            if (String.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            if (String.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var userIdFilter = Builders<DropPoint>.Filter.Where(x => x.UserId == userId);
            var queryFilter = Builders<DropPoint>.Filter.Regex(x => x.Address.Address, new BsonRegularExpression(query, "i"));
            var nameFilter = Builders<DropPoint>.Filter.Regex(x => x.Name, new BsonRegularExpression(query, "i"));
            var nameOrQueryFilter = Builders<DropPoint>.Filter.Or(nameFilter, queryFilter);

            var result = await Collection.Find(Builders<DropPoint>.Filter.And(userIdFilter, nameOrQueryFilter)).ToListAsync();
            return result;
        }
    }
}