namespace TaskCat.Job
{
    using Data.Entity;
    using Common.Db;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Threading.Tasks;

    public class LocalityService : ILocalityService
    {
        private readonly IDbContext _context;

        public LocalityService(IDbContext context)
        {
            this._context = context;
            this.Collection = context.Localities;
        }

        public IMongoCollection<Locality> Collection { get; }

        public async Task RefreshLocalities()
        {
            IAsyncCursor<BsonDocument> outCursor = null;

            try
            {
                outCursor = await this.FetchRefreshedLocalityCursor();
            }
            finally
            {
                outCursor?.Dispose();
            }
        }

        private async Task<IAsyncCursor<BsonDocument>> FetchRefreshedLocalityCursor()
        {
            /*
             * **** GFETCH - 325 *****
             * INFO: This one is particularly ugly. What we want to achieve here:
             * 1. Get locality field from Order payload field From and To
             * 2. Have a distinct set of from and to and return it back as all possible localities
             * 
             * How it should be done.
             * The "refresh" method: The purpose of this method is to refresh all possible locality 
             * in all the jobs. The technique we could use here is an aggregation pipiline. 
             * 
             * In the first aggregation stage we will have all the documents projecting the locality fields
             * in From and To fields in order. 
             * 
             * The second stage would $unwind this new Localities array that we created before so we will have a 
             * basic list of string that have nothing but localities. 
             * 
             * The third stage would make the list unique. We would group them by themselves and since they are
             * just strings now, we will have only the distinct ones.
             * 
             * The fourth stage is a match operation which will essentially map out the empty and the null values 
             * from the pipeline.
             * 
             * The fifth and final stage of aggregation will copy the result to another collection so we have the result 
             * cached in another collection and we will only serve that collection when someone asks for all the localities.
             * 
             * That collection will be manually update only when someone updates a job/creates a job and sees the locality 
             * is new here. It should always be done though a Rx subject of course. We don't want the request thread to be slow.
             */

            var localitiesProperty = "Localities";
            ProjectionDefinition<Data.Entity.Job, BsonDocument> projection = new BsonDocument() {
                { "_id", 0},
                { localitiesProperty, new BsonArray() {
                    "$Order.From.Locality",
                    "$Order.To.Locality" }
                }
            };

            var groupDefinition = BsonDocument.Parse("{ _id : \"$Localities\" }");
            var matchDefinition = BsonDocument.Parse("{ $and:[ { _id : { $ne: null } }, { _id : { $ne: \"\" } }, { _id : { $ne: \"Undefined\" } } ] }");

            /* INFO: Since the outAsync will expose a cursor to the newly created collection
             * and since we are not doing anything with that here, it's only for the better we dispose
             * this now.
             * */

            var outCursor = await _context.Jobs
                .Aggregate()
                .Project(projection)
                .Unwind(localitiesProperty)
                .Group(groupDefinition)
                .Match(matchDefinition)
                .Sort(Builders<BsonDocument>.Sort.Ascending("_id"))
                .OutAsync(CollectionNames.LocalityCollectionName);

            return outCursor;
        }
    }
}
