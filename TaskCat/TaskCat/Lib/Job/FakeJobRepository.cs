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
    using TaskCat.Model.Pagination;
    using System.Web.Http.Controllers;
    using System.Net.Http;
    using MongoDB.Driver;

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

        public Task<IQueryable<Job>> GetJobs(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Job>> GetJobs(string type, int start, int limit)
        {
            throw new NotImplementedException();
        }

        public Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int start, int limit, HttpRequestMessage context)
        {
            throw new NotImplementedException();
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

        public Task<UpdateResult> UpdateJobTask(Job job, JobTask selectedTask)
        {
            throw new NotImplementedException();
        }
    }
}