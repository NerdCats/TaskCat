using MongoDB.Bson.Serialization.Attributes;

namespace TaskCat.Data.Entity
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public class HRIDEntity : DbEntity
    {
        public string HRID { get; set; }
    }
}
