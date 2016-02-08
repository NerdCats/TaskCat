namespace TaskCat.Lib.Order
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;
    using TaskCat.Lib.Db;
    using System.Web.Http;
    using MongoDB.Bson;
    using MongoDB.Driver;

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