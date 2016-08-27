namespace TaskCat.Lib.Utility.Converter
{
    using Data.Model.Vendor.ProfitSharing;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;

    public class VendorProfileConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ProfitSharingStrategy);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            ProfitSharingStrategy strategy;

            var method = obj["Method"];
            if (method == null)
            {
                throw new ArgumentNullException("Type", "Order type is null");
            }

            var profitSharingMethod = (ProfitSharingMethod)Enum.Parse(typeof(ProfitSharingMethod), method.Value<string>());
            switch (profitSharingMethod)
            {
                case ProfitSharingMethod.FLAT_RATE:
                    strategy = new FlatRateStrategy();
                    break;
                case ProfitSharingMethod.PRICE_PERCENTAGE:
                    strategy = new PricePercentageStrategy();
                    break;
                case ProfitSharingMethod.PRICE_PERCENTAGE_WITH_FLAT_RATE:
                    strategy = new PricePercentageStrategy();
                    break;
                default:
                    throw new NotImplementedException($"{profitSharingMethod.ToString()} is invalid/not supported");
            }

            serializer.Populate(obj.CreateReader(), strategy);
            return strategy;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}