namespace TaskCat.Data.Entity
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class WarehouseItem: DbEntity
    {

    }
}
