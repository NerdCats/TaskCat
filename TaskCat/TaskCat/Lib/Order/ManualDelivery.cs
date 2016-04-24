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

        public decimal CalculateServiceCharge()
        {
            decimal ServiceCharge = 0.0M;

            if(Items != null)
            {
                decimal TotalWeight = 0.0M;
                foreach(ItemDetails item in Items)
                {
                    TotalWeight += item.Weight;
                }

                if(TotalWeight <= 1.0M)
                {
                    ServiceCharge = 150.0M;
                }
                else
                {
                    //TODO Calculate service charge for per 500g weight

                }

            }


            return ServiceCharge;
        }
    }
}