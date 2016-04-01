namespace TaskCat.Data.Entity
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class DbEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
    }
}
