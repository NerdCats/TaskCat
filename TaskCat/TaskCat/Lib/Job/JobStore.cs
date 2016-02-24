﻿namespace TaskCat.Lib.Job
{
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Data.Entity;
    using TaskCat.Lib.Db;
    using TaskCat.Data.Model;
    using System.Linq;
    
    public class JobStore
    {
        private IDbContext _context;
        public JobStore(IDbContext context)
        {
            _context = context;
        }
        internal async Task<Data.Entity.Job> CreateOne(Data.Entity.Job createdJob)
        {
            await _context.Jobs.InsertOneAsync(createdJob);
            return createdJob;
        }

        internal async Task<Data.Entity.Job> FindOne(string id)
        {
            var job = await _context.Jobs.Find(x => x._id == id).FirstOrDefaultAsync();
            return job;
        }

        internal async Task<IEnumerable<Data.Entity.Job>> FindJobs(string orderType, int start, int limit)
        {
            var FindContext = string.IsNullOrWhiteSpace(orderType) ? 
                _context.Jobs.Find(x => true) : _context.Jobs.Find(x => x.Order.Type == orderType);
            return await FindContext.SortBy(x => x.CreateTime).Skip(start).Limit(limit).ToListAsync();
        }

        internal async Task<IQueryable<Data.Entity.Job>> FindJobs(int start, int limit)
        {
            return await Task.Run(() => {
                return _context.Jobs.AsQueryable();
            });
        }

        internal async Task<long> CountJobs()
        {
            return await _context.Jobs.CountAsync(x => true);
        }

        internal async Task<UpdateResult> UpdateJobTask(string jobId, int taskIndex, JobTask task)
        {
            task.ModifiedTime = DateTime.Now;
            var Filter = Builders<Data.Entity.Job>.Filter.Where(x => x._id == jobId);
            var UpdateDefinition = Builders<Data.Entity.Job>.Update.Set(x => x.Tasks[taskIndex], task);

            var result = await _context.Jobs.UpdateOneAsync(Filter, UpdateDefinition);
            return result;
        }

        internal async Task<UpdateResult> UpdateJobTasks(string jobId, List<JobTask> tasks)
        {
            var Filter = Builders<Data.Entity.Job>.Filter.Where(x => x._id == jobId);
            var UpdateDefinition = Builders<Data.Entity.Job>.Update.Set(x => x.Tasks, tasks);

            var result = await _context.Jobs.UpdateOneAsync(Filter, UpdateDefinition);
            return result;
        }

        internal async Task<ReplaceOneResult> ReplaceOne(Job job)
        {
            var Filter = Builders<Job>.Filter.Where(x => x._id == job._id);
            var result = await _context.Jobs.ReplaceOneAsync(Filter, job);
            return result;
        }
    }
}