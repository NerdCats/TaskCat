namespace TaskCat.Data.Model.Order.Delivery
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class DeliveryOrder : OrderModel
    {
        /// <summary>
        /// Note to delivery man to provide extra info to delivery man
        /// </summary>
        public string NoteToDeliveryMan { get; set; }

        public decimal RequiredChangeFor { get; set; }

        public DeliveryOrder(string name = null, string type = OrderTypes.Delivery) : base(name, type)
        {
            if (!(type == OrderTypes.Delivery || type == OrderTypes.ClassifiedDelivery))
            {
                throw new InvalidOperationException($"{OrderTypes.Delivery} or {OrderTypes.ClassifiedDelivery} is expected in the process here");
            }
        }
    }
}
