
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

        public UpdateDefinition<BsonDocument> GetReferenceInvoiceId()
        {
            return Builders<BsonDocument>.Update
                .Set("Order.ReferenceInvoiceId", "");
        }

        // Invoice id from old job is still unable to fetch. Need to fix.
        public override void Update()
        {               
            UpdateDefinition<BsonDocument> update = GetReferenceInvoiceId();
            Database.GetCollection<BsonDocument>("Jobs")
                .UpdateMany(x => true, update);

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
