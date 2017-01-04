namespace TaskCat.Job
{
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Entity;
    using Data.Model;
    using System.Linq;
    using Data.Model.Operation;
    using MongoDB.Driver.Linq;
    using Common.Db;
    using MongoDB.Bson;

    public class JobStore
    {
        private IDbContext _context;
        private IMongoQueryable<Job> _jobs;

        public JobStore(IDbContext context)
        {
            _context = context;
            _jobs = context.Jobs.AsQueryable();
        }
        internal async Task<Job> CreateOne(Job createdJob)
        {
            await _context.Jobs.InsertOneAsync(createdJob);
            return createdJob;
        }

        internal async Task<Job> FindOne(string id)
        {
            var job = await _context.Jobs.Find(x => x.Id == id).FirstOrDefaultAsync();
            return job;
        }

        internal async Task<Job> FindOneByHRID(string hrid)
        {
            var job = await _context.Jobs.Find(x => x.HRID == hrid).FirstOrDefaultAsync();
            return job;
        }

        internal async Task<IEnumerable<Job>> FindJobs(string orderType, int start, int limit)
        {
            var FindContext = string.IsNullOrWhiteSpace(orderType) ?
                _context.Jobs.Find(x => true) : _context.Jobs.Find(x => x.Order.Type == orderType);
            return await FindContext.SortBy(x => x.CreateTime).Skip(start).Limit(limit).ToListAsync();
        }

        internal async Task<QueryResult<Job>> FindJobs(string userId, int start, int limit, DateTime? fromDateTime, JobState jobStateToFetchUpTo, SortDirection sortByCreateTimeDirection)
        {
            var FindContext = fromDateTime == null ?
                _context.Jobs.Find(x => x.Assets.ContainsKey(userId) && x.State == jobStateToFetchUpTo) :
                _context.Jobs.Find(x => x.Assets.ContainsKey(userId) && x.State == jobStateToFetchUpTo && x.CreateTime >= fromDateTime);
            var orderContext = sortByCreateTimeDirection == SortDirection.Descending ? FindContext.SortByDescending(x => x.CreateTime) : FindContext.SortBy(x => x.CreateTime);
            return new QueryResult<Job>()
            {
                Total = await orderContext.CountAsync(),
                Result = await orderContext.Skip(start).Limit(limit).ToListAsync()
            };
        }

        internal IQueryable<Job> FindAllAsIQueryable()
        {
            return _context.Jobs.AsQueryable();
        }

        internal async Task<long> CountJobs()
        {
            return await _context.Jobs.CountAsync(x => true);
        }

        internal async Task<Job> UpdateJobTask(string jobId, int taskIndex, JobTask task)
        {
            task.ModifiedTime = DateTime.Now;
            var Filter = Builders<Job>.Filter.Where(x => x.Id == jobId);
            var UpdateDefinition = Builders<Job>.Update.Set(x => x.Tasks[taskIndex], task);

            var result = await _context.Jobs.FindOneAndUpdateAsync(Filter, UpdateDefinition);
            return result;
        }

        internal async Task<Job> UpdateJobTasks(string jobId, List<JobTask> tasks)
        {
            var Filter = Builders<Job>.Filter.Where(x => x.Id == jobId);
            var UpdateDefinition = Builders<Job>.Update.Set(x => x.Tasks, tasks);

            var result = await _context.Jobs.FindOneAndUpdateAsync(Filter, UpdateDefinition);
            return result;
        }

        private class ProjectedLocalities
        {
            public string[] Localities { get; set; }
        };
        internal async Task RefreshJobLocalities()
        {
            /*
             **** GFETCH - 325 *****
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

            ProjectionDefinition<Job, BsonDocument> projection = new BsonDocument() {
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

            IAsyncCursor<BsonDocument> outCursor = default(IAsyncCursor<BsonDocument>);
            try
            {
                outCursor = await _context.Jobs
                    .Aggregate()
                    .Project(projection)
                    .Unwind(localitiesProperty)
                    .Group(groupDefinition)
                    .Match(matchDefinition)
                    .OutAsync(CollectionNames.LocalityCollectionName);
            }
            finally
            {
                if (outCursor != null)
                    outCursor.Dispose();
            }    
        }

        internal async Task<Job> ReplaceOne(Job job)
        {
            job.ModifiedTime = DateTime.UtcNow;
            var Filter = Builders<Job>.Filter.Where(x => x.Id == job.Id);

            var result = await _context.Jobs.FindOneAndReplaceAsync(Filter, job);
            return result;
        }
    }
}