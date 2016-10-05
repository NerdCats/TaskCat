namespace TaskCat.Migration
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoMigrations;

    public class OrderTypeRenameToVariantMigration : CollectionMigration
    {
        public OrderTypeRenameToVariantMigration() : base("1.0.0", "Jobs")
        {
            Description = "Rename Order.PayloadType to Order.Variant";
        }

        public override void UpdateDocument(IMongoCollection<BsonDocument> collection, BsonDocument document)
        {
            UpdateDefinition<BsonDocument> update = GetRenameDefinition();
            var result = collection.UpdateOne(x => true, update);
        }

        public UpdateDefinition<BsonDocument> GetRenameDefinition()
        {
            return Builders<BsonDocument>.Update
                .Rename("Order.PayloadType", "Order.Variant");
        }
    }
}
