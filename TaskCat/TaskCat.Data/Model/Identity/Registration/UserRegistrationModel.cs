namespace TaskCat.Data.Model.Identity.Registration
{
    using Geocoding;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Profile;

    public class UserRegistrationModel : RegistrationModelBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Gender Gender { get; set; }

        public UserRegistrationModel()
        {
        }
    }
}
