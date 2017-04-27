namespace TaskCat.Data.Entity
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System.ComponentModel.DataAnnotations;

    public class DataTag
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9]*(?:_[A-Za-z0-9]+)*$", ErrorMessage = "Tags should start with a letter")]
        public string Id { get; set; }
    }
}
