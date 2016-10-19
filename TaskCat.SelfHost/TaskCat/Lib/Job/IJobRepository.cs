namespace TaskCat.Lib.Job
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Entity;
    using Data.Model.Api;
    using Model.Pagination;
    using System.Net.Http;
    using Data.Model;
    using MongoDB.Driver;
    using Marvin.JsonPatch;
    using System.Linq;
    using Data.Model.Operation;
    using Model.Job;
    public interface IJobRepository
    {
        Task<Job> GetJob(string id);
        Task<IQueryable<Job>> GetJobs();
        Task<Job> GetJobByHrid(string id);
        Task<Job> PostJob(JobModel model);
        Task<IEnumerable<Job>> GetJobs(string type, int start, int limit);
        Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int start, int limit, HttpRequestMessage message);
        Task<ReplaceOneResult> UpdateJob(Job job);
        Task<bool> ResolveAssetRef(JsonPatchDocument<JobTask> taskPatch, JobTask jobtask);
        Task<ReplaceOneResult> UpdateJobTaskWithPatch(string JobId, string taskId, JsonPatchDocument<JobTask> taskPatch);
        Task<ReplaceOneResult> Claim(string jobId, string userId);
        Task<ReplaceOneResult> UpdateOrder(Job job, OrderModel orderModel);
        Task<UpdateResult<Job>> CancelJob(JobCancellationRequest request);
        Task<UpdateResult<Job>> RestoreJob(string jobId);
    }
}
