namespace TaskCat.Lib.Converters
{
    using Data.Model.Identity;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class UserModelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UserModelConverter);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            UserModel model;

            var type = obj["AssetType"];
            if (type == null)
            {
                throw new ArgumentNullException("AssetType", "Asset type is null");
            }

            string modelType = type.Value<string>();
            AssetTypes actualType;
            if (!Enum.TryParse<AssetTypes>(modelType, out actualType))
                throw new NotSupportedException("Invalid AssetType Provided");

            switch (actualType)
            {
                case AssetTypes.FETCHER:
                    model = new UserModel();
                    break;
                default:
                    model = new AssetModel();
                    break;
            }

            serializer.Populate(obj.CreateReader(), model);
            return model;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}