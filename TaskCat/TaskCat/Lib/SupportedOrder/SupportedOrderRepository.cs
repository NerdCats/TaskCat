using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskCat.Lib.SupportedOrder
{
    public class SupportedOrderRepository
    {
        private SupportedOrderStore _store;
        public SupportedOrderRepository(SupportedOrderStore store)
        {
            this._store = store;
        }

        

    }
}