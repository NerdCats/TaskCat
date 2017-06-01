namespace TaskCat.Data.Entity
{
    using MongoDB.Bson.Serialization.Attributes;
    using System.ComponentModel.DataAnnotations;

    public class DataTag : DbEntity
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Data tag value is not provided")]
        [BsonRequired]
        public string Value { get; set; }
    }
}
