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

        public async Task<DropPoint> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = (await Collection.Find(x => x.Id == id).ToListAsync()).FirstOrDefault();
            if (result == null)
            {
                throw new EntityNotFoundException(typeof(DropPoint), id);
            }
            return result;
        }

        public async Task<DropPoint> Get(string id, string userId)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var result = (await Collection.Find(x => x.Id == id && x.UserId == userId).ToListAsync()).FirstOrDefault();
            if (result == null)
            {
                throw new EntityNotFoundException(string.Format("DropPoint with user id {0} and id {1} is invalid", userId, id));
            }
            return result;
        }

        public async Task<DropPoint> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id);
            if (result == null)
                throw new EntityDeleteException(typeof(DropPoint), result.Id);
            return result;
        }

        public async Task<DropPoint> Delete(string id, string userId)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id && x.UserId == userId);
            if (result == null)
                throw new EntityDeleteException(string.Format("DropPoint with user id {0} and id {1} is invalid", userId, id));
            return result;
        }

        public async Task<DropPoint> Insert(DropPoint obj)
        {
            await Collection.InsertOneAsync(obj);
            return obj;
        }

        public async Task<DropPoint> Update(DropPoint obj)
        {
            if (String.IsNullOrWhiteSpace(obj.Id))
            {
                throw new ArgumentNullException(nameof(obj.Id));
            }
            var result = await Collection.FindOneAndReplaceAsync(x => x.Id == obj.Id, obj);
            if (result == null)
                throw new EntityUpdateException(typeof(DropPoint), obj.Id);
            return result;
        }

        public async Task<DropPoint> Update(DropPoint obj, string userId)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (String.IsNullOrWhiteSpace(obj.Id))
            {
                throw new ArgumentNullException(nameof(obj.Id));
            }

            var result = await Collection.FindOneAndReplaceAsync(x => x.Id == obj.Id && x.UserId == userId, obj);
            if (result == null)
                throw new EntityUpdateException(string.Format("DropPoint with user id {0} and id {1} is invalid", userId, obj.Id));
            return result;
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