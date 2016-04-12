namespace TaskCat.Data.Model.Identity.Response
{
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [BsonKnownTypes(typeof(UserModel), typeof(AssetModel), typeof(EnterpriseUserModel))]
    public class UserModelBase
    {
        public bool IsUserAuthenticated { get; set; } = false;
        [JsonProperty(PropertyName = "Id")]
        public virtual string UserId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual IdentityTypes Type { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
    }
}
