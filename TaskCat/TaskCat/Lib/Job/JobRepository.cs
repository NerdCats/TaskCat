namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Data.Entity;
    using Data.Model.Api;

    public class JobRepository : IJobRepository
    {
        private JobManager _manager;

        public JobRepository(JobManager manager)
        {
            _manager = manager;
        }

        public async Task<Job> GetJob(string id)
        {
            return await _manager.GetJob(id);
        }

        public Task<Job> PostJob(JobModel model)
        {
            throw new NotImplementedException();
        }
    }
}