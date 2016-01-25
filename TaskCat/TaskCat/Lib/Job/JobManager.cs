namespace TaskCat.Lib.Job
{
    using Data.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;

    public class JobManager
    {
        private JobStore _store;

        public JobManager(JobStore store)
        {
            _store = store;
        }

        internal async Task<JobEntity> GetJob(string id)
        {
            var JobPayload = await _store.FindOne(id);
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

        internal async Task<IEnumerable<JobEntity>> GetJobs(string type, int start, int limit)
        {
            var jobs = await _store.FindJobs(type, start, limit);
            return jobs;
        }

        internal async Task<JobEntity> RegisterJob(JobEntity createdJob)
        {
            var job = await _store.CreateOne(createdJob);
            return job;
        }
    }
}