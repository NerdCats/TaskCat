namespace TaskCat.Job.Utility.Converter
{
    using System;
    using Newtonsoft.Json;
    using Data.Model.Vendor.ProfitSharing;
    using Newtonsoft.Json.Linq;

    public class ProfitSharingModelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ProfitSharingStrategy);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            ProfitSharingStrategy profitSharingStgy;

            var method = obj["Method"];
            if (method == null)
            {
                throw new ArgumentNullException("Method", "ProfitSharingMethod is null");
            }

            var profitSharingMethod = method.Value<ProfitSharingMethod>();
            switch (profitSharingMethod)
            {
                case ProfitSharingMethod.FLAT_RATE:
                    profitSharingStgy = new FlatRateStrategy();
                    break;
                case ProfitSharingMethod.PRICE_PERCENTAGE:
                    profitSharingStgy = new PricePercentageStrategy();
                    break;
                default:
                    throw new NotSupportedException(string.Concat("Profit Sharing Method invalid/not supported - ", profitSharingMethod));
            }

            serializer.Populate(obj.CreateReader(), profitSharingStgy);
            return profitSharingStgy;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}