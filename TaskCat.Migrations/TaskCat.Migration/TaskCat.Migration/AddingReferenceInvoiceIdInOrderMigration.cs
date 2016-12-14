
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

        public override void Update()
        {
            
        }
    }
}
