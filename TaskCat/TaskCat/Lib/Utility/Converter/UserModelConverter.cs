namespace TaskCat.Lib.Utility.Converter
{
    using Data.Model.Identity;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Data.Model.Identity.Registration;

    public class UserModelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UserRegistrationModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            UserRegistrationModel model;

            var type = obj["AssetType"];
            if (type == null)
            {
                obj["AssetType"] = ((IdentityTypes)0).ToString();
            }

            string modelType = type.Value<string>();
            IdentityTypes actualType;
            if (!Enum.TryParse<IdentityTypes>(modelType, out actualType))
                throw new NotSupportedException("Invalid AssetType Provided");

            switch (actualType)
            {
                case IdentityTypes.FETCHER:
                    model = new UserRegistrationModel();
                    break;
                default:
                    model = new AssetRegistrationModel();
                    break;
            }

            serializer.Populate(obj.CreateReader(), model);
            return model;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}