namespace TaskCat.Data.Model.Identity.Profile
{
    using Geocoding;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements(Inherited = true)]
    [BsonKnownTypes(typeof(UserProfile), typeof(EnterpriseUserProfile))]
    public abstract class IdentityProfile
    {
        public DefaultAddress Address { get; set; }
        public string PicUri { get; set; }

        public abstract string GetName();
    }
}
