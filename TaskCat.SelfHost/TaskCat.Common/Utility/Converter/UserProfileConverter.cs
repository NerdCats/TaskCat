namespace TaskCat.Common.Utility.Converter
{
    using Data.Model.Identity;
    using Data.Model.Identity.Profile;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;

    public class UserProfileConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IdentityProfile);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            IdentityProfile model;

            var type = obj["Type"];
            if (type == null)
            {
                type = ((IdentityTypes)0).ToString();
            }
            else
            {
                obj["Type"].Remove();
            }

           
            string modelType = type.Value<string>();
            IdentityTypes actualType;
            if (!Enum.TryParse<IdentityTypes>(modelType, out actualType))
                throw new NotSupportedException("Invalid AssetType Provided");

            switch (actualType)
            {
                case IdentityTypes.USER:
                    model = new UserProfile();
                    break;
                case IdentityTypes.ENTERPRISE:
                    model = new EnterpriseUserProfile();
                    break;
                default:
                    model = new AssetProfile();
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