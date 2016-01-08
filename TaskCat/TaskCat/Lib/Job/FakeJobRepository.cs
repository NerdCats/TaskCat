namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;

    public class FakeJobRepository : IJobRepository
    {
        public async Task<Job> GetJob(string id)
        {
            var task =  Task.Run(()=> {
                return new Job() {
                    _id = "a1b2c3d4e5f6",
                    Tasks = new List<Task>()
                };
            });
            return await task;
        }
    }
}