namespace TaskCat.Lib.Utility.Converter
{
    using Data.Model.Identity;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using Data.Model.Identity.Registration;

    public class UserRegistrationModelConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UserRegistrationModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            UserRegistrationModel model;

            var type = obj["Type"];
            if (type == null)
            {
                obj["Type"] = ((IdentityTypes)0).ToString();
                type = obj["Type"];
            }

            string modelType = type.Value<string>();
            IdentityTypes actualType;
            if (!Enum.TryParse<IdentityTypes>(modelType, out actualType))
                throw new NotSupportedException("Invalid AssetType Provided");

            switch (actualType)
            {
                case IdentityTypes.USER:
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