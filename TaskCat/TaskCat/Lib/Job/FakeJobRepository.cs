namespace TaskCat.Lib.Job
{
    using Data.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;
    using Data.Model.Api;

    public class FakeJobRepository : IJobRepository
    {
        public async Task<JobEntity> GetJob(string id)
        {
            var task =  Task.Run(()=> {
                return new JobEntity() {
                    _id = "a1b2c3d4e5f6",
                    Tasks = new List<JobTask>()
                };
            });
            return await task;
        }

        public async Task<JobEntity> PostJob(JobModel model)
        {
            var task = Task.Run(() =>
            {
                var job = new JobEntity();
                job.Tasks = model.Tasks;
                job.Name = model.Name;
                return job;
            });

            return await task;
        }
    }
}