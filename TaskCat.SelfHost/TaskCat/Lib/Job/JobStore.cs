namespace TaskCat.Lib.Job
{
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Entity;
    using Db;
    using Data.Model;
    using System.Linq;
    using System.Web.OData.Query;
    using Data.Model.Operation;
    using LinqToQuerystring;
    using MongoDB.Driver.Linq;

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

        internal async Task<QueryResult<Job>> FindJobs(ODataQueryOptions<Job> query, int start, int limit)
        {
            return await Task.Run(() =>
            {
                IQueryable queryResult;
                queryResult = query.ApplyTo(_jobs, AllowedQueryOptions.OrderBy);
                if (query.OrderBy != null)
                    queryResult = queryResult.LinqToQuerystring(typeof(Job), "$orderby=" + query.OrderBy.RawValue) as IQueryable<Job>;

                var data = queryResult as IEnumerable<Job>;

                if (query.Count != null && query.Count.Value)
                    return new QueryResult<Job>(null, data.LongCount());
                else
                    return new QueryResult<Job>(data.Skip(start).Take(limit), data.LongCount());
            });
        }

        internal IQueryable<Job> FindAllAsIQueryable()
        {
            return _context.Jobs.AsQueryable();
        }

        internal async Task<long> CountJobs()
        {
            return await _context.Jobs.CountAsync(x => true);
        }

        internal async Task<UpdateResult> UpdateJobTask(string jobId, int taskIndex, JobTask task)
        {
            task.ModifiedTime = DateTime.Now;
            var Filter = Builders<Job>.Filter.Where(x => x.Id == jobId);
            var UpdateDefinition = Builders<Job>.Update.Set(x => x.Tasks[taskIndex], task);

            var result = await _context.Jobs.UpdateOneAsync(Filter, UpdateDefinition);
            return result;
        }

        internal async Task<UpdateResult> UpdateJobTasks(string jobId, List<JobTask> tasks)
        {
            var Filter = Builders<Job>.Filter.Where(x => x.Id == jobId);
            var UpdateDefinition = Builders<Job>.Update.Set(x => x.Tasks, tasks);

            var result = await _context.Jobs.UpdateOneAsync(Filter, UpdateDefinition);
            return result;
        }

        internal async Task<ReplaceOneResult> ReplaceOne(Job job)
        {
            var Filter = Builders<Job>.Filter.Where(x => x.Id == job.Id);
            var result = await _context.Jobs.ReplaceOneAsync(Filter, job);
            return result;
        }
    }
}