namespace TaskCat.Migration
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoMigrations;

    public class OrderTypeRenameToVariantMigration : Migration
    {
        public OrderTypeRenameToVariantMigration() : base("1.0.0")
        {
            Description = "Rename Order.PayloadType to Order.Variant";
        }

        public UpdateDefinition<BsonDocument> GetRenameDefinition()
        {
            return Builders<BsonDocument>.Update
                .Rename("Order.PayloadType", "Order.Variant");
        }

        public override void Update()
        {
            UpdateDefinition<BsonDocument> update = GetRenameDefinition();
            Database.GetCollection<BsonDocument>("Jobs").UpdateMany(x => true, update);
        }
    }
}
