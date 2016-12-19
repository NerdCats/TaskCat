namespace TaskCat.Migration
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoMigrations;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class OrderReferenceInvoiceIdPopulationMigration : CollectionMigration
    {
        public OrderReferenceInvoiceIdPopulationMigration() : base("1.0.1", "Jobs")
        {
            Description = "Added a field named ReferenceInvocieId in the OrderModel";
            this.Filter = Builders<BsonDocument>.Filter.Eq("Order.Type", "ClassifiedDelivery");
        }

        public BsonDocument AddReferenceInvoiceId(BsonDocument document)
        {
            var order = document["Order"];
            return document;
        }

        public UpdateDefinition<BsonDocument> GenerateUpdateDefinition(string newValue)
        {
            return Builders<BsonDocument>.Update.Set("Order.ReferenceInvoiceId", newValue);
        }

        public override void UpdateDocument(BsonDocument document)
        {
            var firstOrderItemInCart = document["Order"]["OrderCart"]?["PackageList"].AsBsonArray.FirstOrDefault()?["Item"];
            var documentId = document["_id"].ToString();
            var referenceInvoiceId = GetReferenceInvoiceId(firstOrderItemInCart?.ToString());

            // Only update if the value itself is not null, otherwise, don't update database
            if (!string.IsNullOrEmpty(referenceInvoiceId))
            {
                UpdateDefinition<BsonDocument> update = GenerateUpdateDefinition(referenceInvoiceId);
                this.Collection.FindOneAndUpdate(Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(documentId)), update);
                Console.WriteLine($"Updated document -> {document["_id"]}");
            }
        }

        private string GetReferenceInvoiceId(string ItemName)
        {
            if (string.IsNullOrEmpty(ItemName))
                return null;

            Regex regex = new Regex(@"(\w+-\w+)-?\w+-?-?\w+|(I(n|N))?\d+");
            Match match = regex.Match(ItemName);
            if (match.Success)
            {
                return match.Value;
            }
            return null;
        }
    }
}
