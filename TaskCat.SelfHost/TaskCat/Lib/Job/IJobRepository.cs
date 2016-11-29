namespace TaskCat.Lib.Job
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Entity;
    using Data.Model.Api;
    using System.Net.Http;
    using Data.Model;
    using MongoDB.Driver;
    using Marvin.JsonPatch;
    using System.Linq;
    using Data.Model.Operation;
    using Model.Job;
    using Common.Model.Pagination;

    public interface IJobRepository
    {
        Task<Job> GetJob(string id);
        IQueryable<Job> GetJobs();
        Task<Job> GetJobByHrid(string id);
        Task<Job> PostJob(JobModel model);
        Task<IEnumerable<Job>> GetJobs(string type, int start, int limit);
        Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int start, int limit, HttpRequestMessage message);
        Task<UpdateResult<Job>> UpdateJob(Job job);
        Task<UpdateResult<Job>> UpdateJobTaskWithPatch(Job job, string taskId, JsonPatchDocument<JobTask> taskPatch);
        Task<UpdateResult<Job>> Claim(Job job, string userId);
        Task<UpdateResult<Job>> UpdateOrder(Job job, OrderModel orderModel);
        Task<UpdateResult<Job>> CancelJob(Job job, string reason);
        Task<UpdateResult<Job>> RestoreJob(Job job);
    }
}
