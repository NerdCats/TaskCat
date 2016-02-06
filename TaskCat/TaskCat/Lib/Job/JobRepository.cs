﻿namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Data.Entity;
    using Data.Model.Api;
    using Data.Model.Pagination;

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

        public async Task<IEnumerable<Job>> GetJobs(string type, int start, int limit)
        {
            return await _manager.GetJobs(type, start, limit);
        }

        public async Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int start, int limit)
        {
            var data = await GetJobs(type, start, limit);
            var total = await _manager.GetTotalJobCount();
            return new PageEnvelope<Job>(new PaginationHeader()
            {
                Limit = limit,
                Start = start,
                Returned = data.Count(),
                Total = total
            }, data);
        }

        public Task<Job> PostJob(JobModel model)
        {
            throw new NotImplementedException();
        }


    }
}