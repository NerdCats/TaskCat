namespace TaskCat.Lib.Utility.Converter
{
    using Data.Model.Identity;
    using Data.Model.Identity.Profile;
    using Data.Model.Identity.Registration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class UserProfileConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UserProfile);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            UserProfile model;

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
                case IdentityTypes.FETCHER:
                    model = new UserProfile();
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