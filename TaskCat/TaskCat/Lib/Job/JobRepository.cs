﻿namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Data.Entity;
    using Data.Model.Api;
    using TaskCat.Model.Pagination;
    using System.Web.Http.Controllers;
    using System.Web.Http.Routing;
    using System.Net.Http;
    using Constants;

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

        public async Task<IEnumerable<Job>> GetJobs(string type, int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await _manager.GetJobs(type, page * pageSize, pageSize);
        }

        public async Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int page, int pageSize, HttpRequestMessage request)
        {
            var data = await GetJobs(type, page, pageSize);
            var total = await _manager.GetTotalJobCount();

            return new PageEnvelope<Job>(total, page, pageSize, AppConstants.DefaultApiRoute, data, request, new Dictionary<string, object>() { ["type"] = type });
        }

        public Task<Job> PostJob(JobModel model)
        {
            throw new NotImplementedException();
        }


    }
}