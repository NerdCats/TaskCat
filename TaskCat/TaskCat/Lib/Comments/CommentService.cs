namespace TaskCat.Lib.Comments
{
    using System;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Data.Entity;
    using Domain;
    using Db;

    public class ICommentService : IRepository<Comment>
    {
        private IDbContext dbContext;
        public IMongoCollection<Comment> Collection { get; set; }

        public ICommentService(IDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.Collection = dbContext.Comments;
        }

        public Task<Comment> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> Insert(Comment obj)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> Update(Comment obj)
        {
            throw new NotImplementedException();
        }
    }
}