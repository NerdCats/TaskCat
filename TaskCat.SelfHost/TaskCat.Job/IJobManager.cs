namespace TaskCat.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Data.Model;
    using Data.Model.Operation;
    using Data.Entity;

    public interface IJobManager
    {
        Task<Job> GetJob(string id);
        Task<Job> GetJobByHRID(string hrid);
        IQueryable<Job> GetJobs();
        Task<IEnumerable<Job>> GetJobs(string type, int start, int limit);
        Task<QueryResult<Job>> GetJobsAssignedToUser(string userId, int start, int limit, DateTime? fromDateTime, JobState jobStateToFetchUpTo = JobState.IN_PROGRESS, SortDirection sortByCreateTimeDirection = SortDirection.Descending);
        Task<long> GetTotalJobCount();
        Task<Job> RegisterJob(Data.Entity.Job createdJob);
        Task<Job> UpdateJob(Data.Entity.Job job);
        Task<Job> UpdateJobTask(string _id, List<JobTask> tasks);
        Task<Job> UpdateJobTask(string jobId, int taskIndex, JobTask task);
        Task<IEnumerable<string>> GetAllLocalities();
    }
}