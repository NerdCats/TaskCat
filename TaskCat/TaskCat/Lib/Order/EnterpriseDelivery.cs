using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskCat.Data.Model.Inventory;

namespace TaskCat.Lib.Order
{
    public abstract class EnterpriseDelivery : IServiceChargeCalculationService   {
        
        public abstract decimal CalculateServiceCharge();
        
    }
}