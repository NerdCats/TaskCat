
namespace TaskCat.Migration
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoMigrations;
    using System.Text.RegularExpressions;

    public class AddingReferenceInvoiceIdInOrderMigration : Migration
    {
        public AddingReferenceInvoiceIdInOrderMigration() : base("1.0.1")
        {
            Description = "Added a field named ReferenceInvocieId in the OrderModel";
        }

        //public UpdateDefinition<BsonDocument> GetReferenceInvoiceId()
        //{
        //    return Builders<BsonDocument>.Update
        //        .Set("Order.ReferenceInvoiceId", "");
        //}

        protected void UpdateReferenceInvoiceIdValue(IMongoCollection<BsonDocument> collection, BsonDocument document)
        {

            BsonValue itemName;
            document.TryGetValue("Order.OrderCart.PackageList[0].Item", out itemName);
            string extractedInvoiceNumber =  FindInvoiceNumber(itemName.AsString);
            document.Set("Order.ReferenceInvoiceId", extractedInvoiceNumber);
            collection.UpdateOne(x=>true, document);
        }

        // TestJob is a temporary collection of the taskcatdev
        public override async void Update()
        {
            var collection = Database.GetCollection<BsonDocument>("TestJob");
            var filter = new BsonDocument();
            using (var cursor = await collection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        string itemName = "";
                        document.GetValue("Order.OrderCart.PackageList[0].Item", itemName);
                        string extractedInvoiceNumber = FindInvoiceNumber(itemName);
                        System.Console.WriteLine(extractedInvoiceNumber);
                        if(extractedInvoiceNumber == null)
                        {
                            document.Set("Order.ReferenceInvoiceId", "");
                        }
                        else
                        {
                            document.Set("Order.ReferenceInvoiceId", extractedInvoiceNumber);
                        }

                        System.Console.WriteLine(document.AsString);
                        
                        await collection.ReplaceOneAsync(x => true, document);
                    }
                }
            }                
        }

        private string FindInvoiceNumber(string ItemName)
        {
            Regex regex = new Regex(@"(\w+-\w+)-?\w+-?-?\w+|(I(n|N))?\d+");
            Match match = regex.Match(ItemName);
            if(match.Success)
            {
                return match.Value;
            } 
            return null;
        }
    }
}
