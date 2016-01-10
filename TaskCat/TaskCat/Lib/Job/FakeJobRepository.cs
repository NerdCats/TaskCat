namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;
    using Data.Model;
    using Data.Model.Api;

    public class FakeJobRepository : IJobRepository
    {
        public async Task<Job> GetJob(string id)
        {
            var task =  Task.Run(()=> {
                return new Job() {
                    _id = "a1b2c3d4e5f6",
                    Tasks = new List<JobTask>()
                };
            });
            return await task;
        }

        public async Task<Job> PostJob(JobModel model)
        {
            var task = Task.Run(() =>
            {
                var job = new Job();
                job.Tasks = model.Tasks;
                job.Name = model.Name;
                return job;
            });

            return await task;
        }
    }
}