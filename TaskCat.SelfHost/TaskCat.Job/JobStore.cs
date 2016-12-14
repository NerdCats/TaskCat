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

        internal async Task<Job> ReplaceOne(Job job)
        {
            job.ModifiedTime = DateTime.UtcNow;
            var Filter = Builders<Job>.Filter.Where(x => x.Id == job.Id);

            var result = await _context.Jobs.FindOneAndReplaceAsync(Filter, job);
            return result;
        }
    }
}