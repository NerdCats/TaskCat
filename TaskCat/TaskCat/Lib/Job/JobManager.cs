namespace TaskCat.Lib.Job
{
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

            // Hooking up JobTask Predecessors, 
            // validation is skipped due to this is already been recorded in DB
            for(int count = JobPayload.Tasks.Count-1; count>0; count--)
            {
                JobPayload.Tasks[count].SetPredecessor(JobPayload.Tasks[count - 1], false);
            }

            return JobPayload;
        }

        internal async Task<JobEntity> RegisterJob(JobEntity createdJob)
        {
            var job = await _store.CreateOne(createdJob);
            return job;
        }
    }
}