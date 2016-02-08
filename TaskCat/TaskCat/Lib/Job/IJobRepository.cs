namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Entity;
    using Data.Model.Api;
    using TaskCat.Model.Pagination;
    using System.Web.Http.Controllers;
    using System.Net.Http;

    public interface IJobRepository
    {
        Task<Job> GetJob(string id);
        Task<Job> PostJob(JobModel model);
        Task<IEnumerable<Job>> GetJobs(string type, int start, int limit);
        Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int start, int limit, HttpRequestMessage context);
    }
}
