namespace TaskCat.Data.Model.Identity.Registration
{
    using System.Collections.Generic;
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
        public List<string> InterestedLocalities { get; internal set; }

        public UserRegistrationModel()
        {
        }
    }
}
