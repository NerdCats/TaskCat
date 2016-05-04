namespace TaskCat.Lib.Job
{
    using Data.Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Data.Entity;
    using Data.Model.Api;
    using TaskCat.Model.Pagination;
    using System.Net.Http;
    using MongoDB.Driver;
    using Marvin.JsonPatch;
    using System.Web.OData.Query;
    using Data.Model.Query;
    using System.Linq;

    public class FakeJobRepository : IJobRepository
    {
        public async Task<Job> GetJob(string id)
        {
            var task =  Task.Run(()=> {
                return new Job() {
                    Id = "a1b2c3d4e5f6",
                    Tasks = new List<JobTask>()
                };
            });
            return await task;
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

        public Task<bool> ResolveAssetRef(JsonPatchDocument<JobTask> taskPatch)
        {
            throw new NotImplementedException();
        }

        public Task<ReplaceOneResult> UpdateJob(Job job)
        {
            throw new NotImplementedException();
        }

        public Task<ReplaceOneResult> UpdateJobWithPatch(string JobId, string taskId, JsonPatchDocument<JobTask> taskPatch)
        {
            throw new NotImplementedException();
        }

        public Task<ReplaceOneResult> Claim(string jobId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Job> GetJobByHrid(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Job> GetJobByHridOrJobId(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Job>> GetJobs()
        {
            throw new NotImplementedException();
        }

        public Task<ReplaceOneResult> UpdateOrder(string jobId, OrderModel orderModel)
        {
            throw new NotImplementedException();
        }
    }
}