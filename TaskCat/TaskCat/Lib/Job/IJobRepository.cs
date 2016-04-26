namespace TaskCat.Lib.Job
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Data.Entity;
    using Data.Model.Api;
    using TaskCat.Model.Pagination;
    using System.Net.Http;
    using Data.Model;
    using MongoDB.Driver;
    using Marvin.JsonPatch;
    using System.Web.OData.Query;
    using Data.Model.Query;

    public interface IJobRepository
    {
        Task<Job> GetJob(string id);
        Task<Job> GetJobByHrid(string id);
        Task<Job> PostJob(JobModel model);
        Task<IEnumerable<Job>> GetJobs(string type, int start, int limit);
        Task<QueryResult<Job>> GetJobs(ODataQueryOptions<Job> query, int page, int pageSize);
        Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int start, int limit, HttpRequestMessage message);
        Task<PageEnvelope<Job>> GetJobsEnveloped(ODataQueryOptions<Job> query, int page, int pageSize, HttpRequestMessage request);
        Task<ReplaceOneResult> UpdateJob(Job job);
        Task<bool> ResolveAssetRef(JsonPatchDocument<JobTask> taskPatch);
        Task<ReplaceOneResult> UpdateJobWithPatch(string JobId, string taskId, JsonPatchDocument<JobTask> taskPatch);
        Task<ReplaceOneResult> Claim(string jobId, string userId);        
    }
}
