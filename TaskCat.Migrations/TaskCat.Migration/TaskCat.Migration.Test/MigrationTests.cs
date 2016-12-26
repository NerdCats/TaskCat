namespace TaskCat.Migration.Test
{
    using NUnit.Framework;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    [TestFixture]
    public class MigrationTests
    {
        [Test]
        public void MigrationToRenamePayloadTypeToVariant_HasPayloadType_RenamesToVariant()
        {
            var updateFilter = new BsonDocument
            {
                { "$rename", new BsonDocument
                    {
                        {"Order.PayloadType", "Order.Variant" }
                    }
                }
            };       

            var migration = new OrderTypeRenameToVariantMigration();
            var updateDef = migration.GetRenameDefinition();
            
            var result = updateDef.Render(new BsonDocumentSerializer(), new BsonSerializerRegistry());
            Assert.AreEqual(updateFilter, result);
        }

        [Test]
        public void MigrationToPopulateReferenceInvoiceId_IsEnterpriseJob_PopulatesOldJobWithReferenceInvoiceId()
        {
            var migration = new OrderReferenceInvoiceIdPopulationMigration();
            var updateDef = migration.GenerateUpdateDefinition("Something");
            var result = updateDef.Render(new BsonDocumentSerializer(), BsonSerializer.SerializerRegistry);

        }
    }
}
