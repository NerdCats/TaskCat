using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskCat.Data.Model.Inventory;

namespace TaskCat.Lib.Order
{
    public class ZuumZuumDelivery : EnterpriseDelivery
    {
        List<ItemDetails> Items;

        public ZuumZuumDelivery(List<ItemDetails> items)
        {
            this.Items = items;
        }

        public override decimal CalculateServiceCharge()
        {
            throw new NotImplementedException();
        }
    }
}