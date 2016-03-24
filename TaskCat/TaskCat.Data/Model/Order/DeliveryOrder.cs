namespace TaskCat.Data.Model.Order
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DeliveryOrder : OrderModel
    {
        public Location From { get; set; }
        public Location To { get; set; }

        public string PackageDescription { get; set; }
        public double PackageWeight { get; set; }

        public DeliveryOrder(string name = null) : base(name, "Delivery")
        {
        }
    }
}
