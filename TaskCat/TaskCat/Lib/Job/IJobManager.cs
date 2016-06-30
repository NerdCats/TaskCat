using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.OData.Query;
using MongoDB.Driver;
using TaskCat.Data.Entity;
using TaskCat.Data.Model;
using TaskCat.Data.Model.Operation;

namespace TaskCat.Lib.Job
{
    public interface IJobManager
    {
        Task<Data.Entity.Job> GetJob(string id);
        Task<Data.Entity.Job> GetJobByHRID(string hrid);
        Task<IQueryable<Data.Entity.Job>> GetJobs();
        Task<IEnumerable<Data.Entity.Job>> GetJobs(string type, int start, int limit);
        Task<QueryResult<Data.Entity.Job>> GetJobs(ODataQueryOptions<Data.Entity.Job> query, int start, int limit);
        Task<QueryResult<Data.Entity.Job>> GetJobsAssignedToUser(string userId, int start, int limit, DateTime? fromDateTime, JobState jobStateToFetchUpTo = JobState.IN_PROGRESS, SortDirection sortByCreateTimeDirection = SortDirection.Descending);
        Task<long> GetTotalJobCount();
        Task<Data.Entity.Job> RegisterJob(Data.Entity.Job createdJob);
        Task<ReplaceOneResult> UpdateJob(Data.Entity.Job job);
        Task<UpdateResult> UpdateJobTask(string _id, List<JobTask> tasks);
        Task<UpdateResult> UpdateJobTask(string jobId, int taskIndex, JobTask task);
    }
}