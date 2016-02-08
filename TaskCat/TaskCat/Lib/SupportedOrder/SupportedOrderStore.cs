using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskCat.Data.Entity;
using TaskCat.Lib.Db;

namespace TaskCat.Lib.SupportedOrder
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using TaskCat.Data.Entity;
    public class SupportedOrderStore
    {
        private IDbContext _context;

        public SupportedOrderStore(DbContext context)
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
    }
}