using System;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoMigrations;

namespace TaskCat.Migration
{
    public class JobMigration: CollectionMigration
    {
        public JobMigration(MigrationVersion version, string collectionName) : base(version, collectionName)
        {
        }

        public override void UpdateDocument(IMongoCollection<BsonDocument> collection, BsonDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
