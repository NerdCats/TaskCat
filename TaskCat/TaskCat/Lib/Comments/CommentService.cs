namespace TaskCat.Lib.Comments
{
    using System;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Data.Entity;
    using Domain;
    using Db;
    using System.Linq;
    using Exceptions;
    using System.Collections.Generic;

    public class CommentService : IRepository<Comment>
    {
        private IDbContext dbContext;
        public IMongoCollection<Comment> Collection { get; set; }

        public CommentService(IDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.Collection = dbContext.Comments;
        }

        public async Task<Comment> Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            var result = await Collection.FindOneAndDeleteAsync(x => x.Id == id);

            if (result == null)
                throw new EntityDeleteException(typeof(Comment), id);
            return result;
        }

        public async Task<IEnumerable<Comment>> GetByRefId(string id, string entityType, int page, int pageSize)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            if (String.IsNullOrWhiteSpace(entityType))
                throw new ArgumentNullException(nameof(entityType));

            var result = await Collection.Find(x => x.RefId == id && x.EntityType == entityType)
                .SortByDescending(x => x.CreateTime)
                .Skip(page * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            if (result == null)
            {
                throw new EntityNotFoundException(typeof(Comment), id);
            }
            return result;
        }

        public async Task<Comment> Get(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            var result = (await Collection.Find(x => x.Id == id).ToListAsync()).FirstOrDefault();
            if (result == null)
            {
                throw new EntityNotFoundException(typeof(Comment), id);
            }
            return result;
        }

        public async Task<Comment> Insert(Comment obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            obj.CreateTime = DateTime.UtcNow;
            await Collection.InsertOneAsync(obj);
            return obj;
        }

        public async Task<Comment> Update(Comment obj)
        {
            if (String.IsNullOrWhiteSpace(obj.Id)) throw new ArgumentNullException(nameof(obj.Id));

            obj.LastModified = DateTime.UtcNow;
            var result = await Collection.FindOneAndReplaceAsync(x => x.Id == obj.Id, obj);
        
            if (result == null)
                throw new EntityUpdateException(typeof(Comment), obj.Id);
            return result;
        }
    }
}