namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;

    public class JobManager
    {
        private JobStore _store;

        public JobManager(JobStore store)
        {
            _store = store;
        }

        internal async Task<JobEntity> GetJob(string id)
        {
            return await _store.FindOne(id);
        }

        internal async Task<JobEntity> RegisterJob(JobEntity createdJob)
        {
            var job = await _store.CreateOne(createdJob);
            return job;
        }
    }
}