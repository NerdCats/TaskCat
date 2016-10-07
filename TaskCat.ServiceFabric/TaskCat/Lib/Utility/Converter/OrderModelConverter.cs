namespace TaskCat.Lib.Utility.Converter
{
    using Data.Model.Order;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using Data.Model;

    internal class OrderModelConverter : JsonConverter
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
            orderModel = OrderTypeResolver.CreateOrderInstance(orderType);

            serializer.Populate(obj.CreateReader(), orderModel);
            return orderModel;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}