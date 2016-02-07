namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Data.Entity;
    using Data.Model.Api;
    using Data.Model.Pagination;
    using System.Web.Http.Controllers;
    using Utility;
    using System.Web.Http.Routing;
    using System.Net.Http;
    using Constants;

    public class JobRepository : IJobRepository
    {
        private JobManager _manager;
        private PagingHelper _paginationHelper;

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

        public async Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int start, int limit , HttpRequestMessage request)
        {
            _paginationHelper = new PagingHelper(request);

            var data = await GetJobs(type, start, limit);
            var total = await _manager.GetTotalJobCount();

            return new PageEnvelope<Job>(new PaginationHeader(total, start, limit, data.Count())
            {
                NextPage = _paginationHelper.GenerateNextPageUrl(AppConstants.DefaultApiRoute, type, start, limit, total),
                PrevPage = _paginationHelper.GeneratePreviousPageUrl(AppConstants.DefaultApiRoute, type, start, limit, total)
            }, data);
        }

        public Task<Job> PostJob(JobModel model)
        {
            throw new NotImplementedException();
        }


    }
}