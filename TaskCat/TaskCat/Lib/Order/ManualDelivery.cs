using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskCat.Data.Model.Inventory;

namespace TaskCat.Lib.Order
{
    public class ManualDelivery : IServiceChargeCalculationService
    {
        List<ItemDetails> Items;

        public ManualDelivery(List<ItemDetails> items)
        {
            this.Items = items;
        }

        public double CalculateServiceCharge()
        {
            double ServiceCharge;

            if(Items != null)
            {
                foreach(ItemDetails item in Items)
                {

                }
            }


            return ;
        }
    }
}