namespace TaskCat.Lib.DropPoint
{
    using Db;
    using MongoDB.Driver;
    using Data.Entity;
    using Domain;
    using System;
    using System.Threading.Tasks;

    public class DropPointService: IRepository<DropPoint>
    {
        private IMongoCollection<DropPoint> collection;

        public DropPointService(IDbContext dbContext)
        {
            this.collection = dbContext.DropPoints;
        }

        public async Task<DropPoint> Delete(string id)
        {
            var item = await Get(id);
            var result = await collection.DeleteOneAsync(x => x.Id == item.id);
            if (result.DeletedCount > 0 && result.IsAcknowledged)
                return item;
            else

            
        }

        public async Task<DropPoint> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<DropPoint> Insert(DropPoint obj)
        {
            throw new NotImplementedException();
        }

        public async Task<DropPoint> Update(DropPoint obj)
        {
            throw new NotImplementedException();
        }
    }
}