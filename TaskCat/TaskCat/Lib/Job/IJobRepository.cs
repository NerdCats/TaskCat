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
    using System.Linq;

    public interface IJobRepository
    {
        Task<Job> GetJob(string id);
        Task<IQueryable<Job>> GetJobs();
        Task<Job> GetJobByHrid(string id);
        Task<Job> PostJob(JobModel model);
        Task<IEnumerable<Job>> GetJobs(string type, int start, int limit);
        Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int start, int limit, HttpRequestMessage message);
        Task<ReplaceOneResult> UpdateJob(Job job);
        Task<bool> ResolveAssetRef(JsonPatchDocument<JobTask> taskPatch);
        Task<ReplaceOneResult> UpdateJobWithPatch(string JobId, string taskId, JsonPatchDocument<JobTask> taskPatch);
        Task<ReplaceOneResult> Claim(string jobId, string userId);        
    }
}
