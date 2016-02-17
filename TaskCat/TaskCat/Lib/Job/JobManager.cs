namespace TaskCat.Lib.Job
{
    using Data.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;
    using System.Web.Http;
    using MongoDB.Driver;

    public class JobManager
    {
        private JobStore _store;

        public JobManager(JobStore store)
        {
            _store = store;
        }

        internal async Task<Job> GetJob(string id)
        {
            var JobPayload = await _store.FindOne(id);
            if (JobPayload == null)
                return null;
            JobTask TerminalTask=null;
            // Hooking up JobTask Predecessors, 
            // validation is skipped due to this is already been recorded in DB
            for(int count = JobPayload.Tasks.Count-1; count>0; count--)
            {
                JobPayload.Tasks[count].SetPredecessor(JobPayload.Tasks[count - 1], false);
                if (JobPayload.Tasks[count].IsTerminatingTask)
                    TerminalTask = JobPayload.Tasks[count];
            }

            TerminalTask = TerminalTask ?? JobPayload.Tasks[0];
            JobPayload.TerminalTask = TerminalTask;

            return JobPayload;
        }

        internal async Task<long> GetTotalJobCount()
        {
            return await _store.CountJobs();
        }

        internal async Task<IQueryable<Job>> GetJobs(int start, int limit)
        {
            var jobs = await _store.FindJobs(start, limit);
            return jobs;
        }

        internal async Task<IEnumerable<Job>> GetJobs(string type, int start, int limit)
        {
            var jobs = await _store.FindJobs(type, start, limit);
            return jobs;
        }

        internal async Task<Job> RegisterJob(Job createdJob)
        {
            var job = await _store.CreateOne(createdJob);
            return job;
        }

        internal async Task<UpdateResult> UpdateJobTask(string jobId, int taskIndex, JobTask task)
        {
            return await _store.UpdateJobTask(jobId, taskIndex, task);
        }
    }
}