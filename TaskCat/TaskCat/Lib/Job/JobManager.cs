namespace TaskCat.Lib.Job
{
    using Data.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Entity;
    using MongoDB.Driver;
    using System.Web.OData.Query;
    using Data.Model.Operation;
    using System;
    using Exceptions;
    using System.Linq;

    public class JobManager : IJobManager
    {
        private JobStore store;

        public JobManager(JobStore store)
        {
            this.store = store;
        }

        public async Task<IQueryable<Job>> GetJobs()
        {
            return await store.FindAllAsIQueryable();
        }

        public async Task<Job> GetJob(string id)
        {
            var JobPayload = await store.FindOne(id);
            if (JobPayload == null)
                throw new EntityNotFoundException("Job", id);
            return ArrangeJobHooks(JobPayload);
        }

        public async Task<Job> GetJobByHRID(string hrid)
        {
            var JobPayload = await store.FindOneByHRID(hrid);
            if (JobPayload == null)
                throw new EntityNotFoundException("Job", hrid);
            return ArrangeJobHooks(JobPayload);
        }

        private Job ArrangeJobHooks(Job JobPayload)
        {
            JobTask TerminalTask = null;
            // Hooking up JobTask Predecessors, 
            // validation is skipped due to this is already been recorded in DB
            for (int count = JobPayload.Tasks.Count - 1; count > 0; count--)
            {
                JobPayload.Tasks[count].SetPredecessor(JobPayload.Tasks[count - 1], false);
                if (JobPayload.Tasks[count].IsTerminatingTask)
                    TerminalTask = JobPayload.Tasks[count];
            }

            TerminalTask = TerminalTask ?? JobPayload.Tasks[0];
            JobPayload.TerminalTask = TerminalTask;

            JobPayload.EnsureTaskAssetEventsAssigned();
            JobPayload.EnsureAssetModelsPropagated();
            JobPayload.SetupDefaultBehaviourForFirstJobTask();

            return JobPayload;
        }

        public async Task<long> GetTotalJobCount()
        {
            return await store.CountJobs();
        }

        public async Task<QueryResult<Job>> GetJobs(ODataQueryOptions<Job> query, int start, int limit)
        {
            var jobsResult = await store.FindJobs(query, start, limit);
            return jobsResult;
        }

        public async Task<IEnumerable<Job>> GetJobs(string type, int start, int limit)
        {
            var jobs = await store.FindJobs(type, start, limit);
            return jobs;
        }

        public async Task<QueryResult<Job>> GetJobsAssignedToUser(string userId, int start, int limit, DateTime? fromDateTime, JobState jobStateToFetchUpTo = JobState.IN_PROGRESS, SortDirection sortByCreateTimeDirection = SortDirection.Descending)
        {
            var jobs = await store.FindJobs(userId, start, limit, fromDateTime, jobStateToFetchUpTo, sortByCreateTimeDirection);
            return jobs;
        }

        public async Task<Job> RegisterJob(Job createdJob)
        {
            var job = await store.CreateOne(createdJob);
            return job;
        }

        public async Task<UpdateResult> UpdateJobTask(string jobId, int taskIndex, JobTask task)
        {
            return await store.UpdateJobTask(jobId, taskIndex, task);
        }

        public async Task<UpdateResult> UpdateJobTask(string _id, List<JobTask> tasks)
        {
            return await store.UpdateJobTasks(_id, tasks);
        }

        public async Task<ReplaceOneResult> UpdateJob(Job job)
        {
            return await store.ReplaceOne(job);
        }
    }
}