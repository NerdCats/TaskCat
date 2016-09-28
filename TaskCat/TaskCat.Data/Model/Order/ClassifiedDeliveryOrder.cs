namespace TaskCat.Data.Model.Order
{
    using Person;
    using System.ComponentModel.DataAnnotations;

    public class ClassifiedDeliveryOrder : DeliveryOrder
    {
        [Required(ErrorMessage = "Seller info not provided")]
        public PersonInfo SellerInfo { get; set; }

        [Required(ErrorMessage = "Buyer info not provided")]
        public PersonInfo BuyerInfo { get; set; }

        public ClassifiedDeliveryOrder(string name = null, string type = OrderTypes.ClassifiedDelivery)
            : base(name, type)
        {

        }
    }
}
