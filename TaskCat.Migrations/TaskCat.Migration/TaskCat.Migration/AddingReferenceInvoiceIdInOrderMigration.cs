
namespace TaskCat.Migration
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoMigrations;
    using System.Text.RegularExpressions;

    public class AddingReferenceInvoiceIdInOrderMigration : Migration
    {
        public AddingReferenceInvoiceIdInOrderMigration() : base("1.0.0")
        {
            Description = "Added a separate field called ReferenceInvocieId";
        }

        public UpdateDefinition<BsonDocument> GetReferenceInvoiceId()
        {
            return Builders<BsonDocument>.Update.Set("Order.ReferenceInvoiceId", "");
        }

        protected void UpdateReferenceInvoiceIdValue(IMongoCollection<BsonDocument> collection, BsonDocument document)
        {

            BsonValue itemName;
            document.TryGetValue("Order.OrderCart.PackageList[0].Item", out itemName);
            string extractedInvoiceNumber =  FindInvoiceNumber(itemName.AsString);
            document.Set("Order.ReferenceInvoiceId", extractedInvoiceNumber);
            collection.UpdateOne(x=>true, document);
        }

        // TestJob is a temporary collection of the taskcatdev
        public override void Update()
        {
            UpdateDefinition<BsonDocument> update = GetReferenceInvoiceId();
            Database.GetCollection<BsonDocument>("TestJob")
                .UpdateMany(x => true, update);

            //Database.GetCollection<BsonDocument>("TestJob")
            //    .UpdateMany<BsonDocument>((x => true), UpdateReferenceInvoiceIdValue(Database.GetCollection<BsonDocument>("TestJob")));
        }

        private string FindInvoiceNumber(string ItemName)
        {
            Regex regex = new Regex(@"(\w+-\w+)-?\w+-?-?\w+|(I(n|N))?\d+");
            Match match = regex.Match(ItemName);
            if(match.Success)
            {
                return match.Value;
            }
            return "No Reference Invoice Id found for the product";
        }
    }
}
