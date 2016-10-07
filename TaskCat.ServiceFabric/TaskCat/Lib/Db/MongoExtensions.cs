namespace TaskCat.Lib.Db
{
    using AspNet.Identity.MongoDB;
    using Data.Entity;
    using MongoDB.Driver;
    using MongoDB.Bson;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    internal static class MongoExtensions
    {
        public static async Task<IEnumerable<MongoPopulateResult<FDocument, TDocument>>>
            Populate<FDocument, TDocument>(
            this IEnumerable<FDocument> collection,
            Expression<Func<FDocument, string>> fieldExpression,
            IMongoCollection<TDocument> TDocumentCollection)
            where FDocument : DbEntity
            where TDocument : IdentityUser
        {
            //TODO: Should really check whether both of the fields are actually BsonId fields
            var funcToFetchField = fieldExpression.Compile();
            var collectionDict = collection.ToDictionary(x => funcToFetchField(x));

            int count = 0;

            Dictionary<string, MongoPopulateResult<FDocument, TDocument>> result = new Dictionary<string, MongoPopulateResult<FDocument, TDocument>>();

            FilterDefinition<TDocument> filter = Builders<TDocument>.Filter.In(x => x.Id, collectionDict.Keys);
            using (var cursor = await TDocumentCollection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var item in batch)
                    {
                        result[item.Id] = new MongoPopulateResult<FDocument, TDocument>(collectionDict[item.Id], item);
                    }
                    count += batch.Count();
                }
            }

            

            //TODO: Throw exceptions if count match failes
            //TODO: Throw exceptions if the collection is not distinct
            //TODO: Find ways to make it IEnumerable (deferrable)

            return collection.Select(x=>result[funcToFetchField(x)]);
        }

        public static IEnumerable<T> Populate<T>() where T : IdentityUser
        {
            throw new NotImplementedException();
        }
    }

    internal class MongoPopulateResult<FDocument, TDocument>
    {
        public FDocument FDoc { get; set; }
        public TDocument TDoc { get; set; }

        public MongoPopulateResult(FDocument fdoc, TDocument tdoc)
        {
            FDoc = fdoc;
            TDoc = tdoc;
        }
    }
}