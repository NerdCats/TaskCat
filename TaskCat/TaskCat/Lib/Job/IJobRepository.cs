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
    using System.Web.Http;
    using Data.Model;
    using MongoDB.Driver;
    using Marvin.JsonPatch;

    public interface IJobRepository
    {
        Task<Job> GetJob(string id);
        Task<Job> PostJob(JobModel model);
        Task<IEnumerable<Job>> GetJobs(string type, int start, int limit);
        Task<IQueryable<Job>> GetJobs(int page, int pageSize);
        Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int start, int limit, HttpRequestMessage context);
        Task<ReplaceOneResult> UpdateJob(Job job);
        Task<bool> ResolveAssetRef(JsonPatchDocument<JobTask> taskPatch);
        Task<ReplaceOneResult> UpdateJobWithPatch(string JobId, string taskId, JsonPatchDocument<JobTask> taskPatch);
    }
}
