namespace TaskCat.Lib.Utility
{
    using Data.Model.Order;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Model;

    public class OrderModelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OrderModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            OrderModel orderModel;

            var type = obj["Type"];
            if (type == null)
            {
                throw new ArgumentNullException("Type", "Order type is null");
            }

            string orderType = type.Value<string>();
            switch (orderType)
            {
                case "Ride":
                    orderModel = new RideOrder();
                    break;
                default:
                    throw new NotSupportedException(string.Concat("Order Entry type invalid/no supported - ", orderType));
            }

            serializer.Populate(obj.CreateReader(), orderModel);
            return orderModel;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}