namespace TaskCat.Lib.Order
{
    using System.Collections.Generic;
    using Data.Model.Inventory;
    using System.Linq;

    public class DefaultDeliveryServiceChargeCalculationService : IServiceChargeCalculationService
    {
        public decimal CalculateServiceCharge(List<ItemDetails> items)
        {
            decimal ServiceCharge = 0.0M;

            if (items != null)
            {
                decimal TotalWeight = 0.0M;
                TotalWeight = items.Sum(x => x.Weight);

                if (TotalWeight <= 1.0M)
                {
                    ServiceCharge = 150.0M;
                }
                else
                {
                    // TODO: Calculate service charge for per 500g weight                    
                    ServiceCharge = 150.0M;         // Mock value

                }

            }
            else
            {
                return 150;
            }

            return ServiceCharge;
        }
    }
}