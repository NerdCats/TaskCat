namespace TaskCat.Data.Model.Order
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class DeliveryOrder : OrderModel
    {
        /// <summary>
        /// Note to delivery man to provide extra info to delivery man
        /// </summary>
        public string NoteToDeliveryMan { get; set; }

        public decimal RequiredChangeFor { get; set; }

        public DeliveryOrder(string name = null) : base(name, OrderTypes.Delivery)
        {

        }
    }
}
