namespace TaskCat.Data.Model.Order
{
    using Person;
    public class ClassifiedDeliveryOrder : DeliveryOrder
    {
        public PersonInfo SellerInfo { get; set; }
        public PersonInfo BuyerInfo { get; set; }

        public ClassifiedDeliveryOrder(string name = null, string type = OrderTypes.ClassifiedDelivery)
            : base(name, type)
        {

        }
    }
}
