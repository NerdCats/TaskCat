using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using TaskCat.Data.Entity;
using TaskCat.Lib.Db;

namespace TaskCat.Lib.Job
{
    public class JobStore
    {
        private IDbContext _context;
        public JobStore(IDbContext context)
        {
            _context = context;
        }
        internal async Task<JobEntity> CreateOne(JobEntity createdJob)
        {
            await _context.Jobs.InsertOneAsync(createdJob);
            return createdJob;
        }

        internal async Task<JobEntity> FindOne(string id)
        {
            var job = await _context.Jobs.Find(x => x._id == id).FirstOrDefaultAsync();
            return job;
        }
    }
}