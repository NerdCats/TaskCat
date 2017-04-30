namespace TaskCat.Data.Entity
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class DataTag
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
    }
}
