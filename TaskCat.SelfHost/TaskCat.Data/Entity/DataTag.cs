namespace TaskCat.Data.Entity
{
    using MongoDB.Bson.Serialization.Attributes;

    public class DataTag : DbEntity
    {
        [BsonRequired]
        public string Value { get; set; }
    }
}
