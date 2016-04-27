namespace TaskCat.Data.Model.Identity.Profile
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements(Inherited = true)]
    [BsonKnownTypes(typeof(UserProfile), typeof(EnterpriseUserProfile))]
    public class IdentityProfile
    {
        public string Address { get; set; }
        public string PicUri { get; set; }
    }
}
