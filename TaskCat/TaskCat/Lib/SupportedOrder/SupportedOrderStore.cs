using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskCat.Lib.Db;

namespace TaskCat.Lib.SupportedOrder
{
    public class SupportedOrderStore
    {
        private DbContext _context;

        public SupportedOrderStore(DbContext context)
        {
            this._context = context;
        }
    }
}