﻿namespace TaskCat.Lib.Tag
{
    using System;
    using System.Threading.Tasks;
    using TaskCat.Job;
    using Common.Db;
    using MongoDB.Driver;
    using Data.Entity;
    using Common.Exceptions;
    using System.Linq;

    public class TagService : ITagService
    {
        public IMongoCollection<DataTag> Collection { get; set; }

        public TagService(IDbContext dbContext)
        {
            this.Collection = dbContext.Tags;
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

        public async Task<DataTag> Insert(DataTag tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            await Collection.InsertOneAsync(tag);
            return tag;
        }

        public async Task<DataTag> Update(DataTag tag)
        {
            if (string.IsNullOrWhiteSpace(tag.Id)) throw new ArgumentNullException(nameof(tag.Id));

            var result = await Collection.FindOneAndReplaceAsync(x => x.Id == tag.Id, tag);
            if (result == null)
                throw new EntityUpdateException(typeof(DataTag), tag.Id);
            return result;
        }
    }
}
