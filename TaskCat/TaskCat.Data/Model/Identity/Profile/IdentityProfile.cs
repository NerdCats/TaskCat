namespace TaskCat.Data.Model.Identity.Profile
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonKnownTypes(typeof(UserProfile))]
    public class IdentityProfile
    {
        public string Address { get; set; }
        public string PicUri { get; set; }
    }
}
