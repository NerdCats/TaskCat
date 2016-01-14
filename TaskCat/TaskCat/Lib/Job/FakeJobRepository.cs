namespace TaskCat.Lib.Job
{
    using Data.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;

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
    }
}