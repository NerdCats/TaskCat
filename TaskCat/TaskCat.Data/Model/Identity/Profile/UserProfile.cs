namespace TaskCat.Data.Model.Identity.Profile
{
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Registration;
    using Newtonsoft.Json.Converters;

    [BsonIgnoreExtraElements(Inherited = true)]
    [BsonDiscriminator(Required = true)]
    [BsonKnownTypes(typeof(AssetProfile))]
    public class UserProfile : IdentityProfile
    {
        //FIXME: None of these are mandatory by default for a user but we actually can make an attribute to make 
        // it conditionally required, might be a good thought
        // these fields can be populated if signed up through social media !
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public string FullName { get { return string.Concat(FirstName, " ", LastName); } }
        public int Age { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Gender Gender { get; set; }             

        public UserProfile()
        {

        }

        //FIXME: This should be segregated, having the whole usermodel coming here is okay, but not good
        public UserProfile(UserRegistrationModel userModel)
        {
            this.FirstName = userModel.FirstName;
            this.LastName = userModel.LastName;
            this.Gender = userModel.Gender;
            this.Address = userModel.Address;
            this.Age = userModel.Age;
        }
    }
}
