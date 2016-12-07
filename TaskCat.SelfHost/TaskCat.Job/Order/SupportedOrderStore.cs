namespace TaskCat.Job.Order
{
    using Data.Entity;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Common.Db;

    public class SupportedOrderStore
    {
        private IDbContext _context;

        public SupportedOrderStore(IDbContext context)
        {
            this._context = context;
        }

        internal async Task<SupportedOrder> Post(SupportedOrder supportedOrder)
        {
            await _context.SupportedOrders.InsertOneAsync(supportedOrder);
            return supportedOrder;
        }

        internal async Task<List<SupportedOrder>> GettAll()
        {
            return await _context.SupportedOrders.Find(x=>true).ToListAsync();
        }

        internal async Task<SupportedOrder> Get(string id)
        {
            return await _context.SupportedOrders.Find(x => x._id == id).FirstOrDefaultAsync();
        }

        internal async Task<SupportedOrder> Replace(SupportedOrder order)
        {
            var filter = Builders<SupportedOrder>.Filter.Where(x => x._id == order._id);
            return await _context.SupportedOrders.FindOneAndReplaceAsync(filter, order);
        }

        internal async Task<SupportedOrder> Delete(string id)
        {
            var filter = Builders<SupportedOrder>.Filter.Where(x => x._id == id);
            return await _context.SupportedOrders.FindOneAndDeleteAsync(filter);
        }
    }
}