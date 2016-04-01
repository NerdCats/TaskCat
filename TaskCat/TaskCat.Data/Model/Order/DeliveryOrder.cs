namespace TaskCat.Data.Model.Order
{
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
