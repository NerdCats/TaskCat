using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskCat.Data.Entity;

namespace TaskCat.Lib.SupportedOrder
{
    using System.Web.Http;
    using TaskCat.Data.Entity;
    public class SupportedOrderRepository
    {
        private SupportedOrderStore _store;
        public SupportedOrderRepository(SupportedOrderStore store)
        {
            this._store = store;
        }

        internal async Task<SupportedOrder> PostSupportedOrder(SupportedOrder supportedOrder)
        {
            await _store.Post(supportedOrder);
            return supportedOrder;
        }

        internal async Task<List<SupportedOrder>> GetAll()
        {
            return await _store.GettAll();
        }
    }
}