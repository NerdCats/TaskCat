namespace TaskCat.Warehouse.Lib
{
    using System;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using TaskCat.Common.Db;
    using TaskCat.Common.Domain;
    using TaskCat.Data.Entity;
    using System.Collections.Generic;

    public class StockService : IRepository<StockItem>, IStockService
    {
        private IDbContext context;

        public StockService(IDbContext context)
        {
            this.context = context;
        }

        public IMongoCollection<StockItem> Collection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<StockItem> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StockItem>> GetStocksByReference(string referenceId, string refType)
        {
            if (string.IsNullOrWhiteSpace(referenceId))
                throw new ArgumentException($"{referenceId} is null or empty");

            if (string.IsNullOrWhiteSpace(refType))
                throw new ArgumentException($"{refType} is null or empty");

            return await context.StockItems.Find(x => x.RefEntityType == refType && x.RefId == referenceId).ToListAsync();
        }

        public Task<StockItem> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<StockItem> Insert(StockItem obj)
        {
            throw new NotImplementedException();
        }

        public Task<StockItem> Update(StockItem obj)
        {
            throw new NotImplementedException();
        }
    }
}
