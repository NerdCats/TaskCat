namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Data.Entity;
    using Data.Model.Api;
    using TaskCat.Model.Pagination;
    using System.Web.Http.Controllers;
    using System.Web.Http.Routing;
    using System.Net.Http;
    using Constants;
    using System.Web.Http;
    using Data.Model;
    using MongoDB.Driver;
    using Auth;
    using Data.Model.Identity.Response;
    using Marvin.JsonPatch;
    using Newtonsoft.Json;

    public class JobRepository : IJobRepository
    {
        private JobManager _manager;
        private AccountManger _accountManager; // FIXME: When a full fledged assetManager comes up this should be replaced by that

        public JobRepository(JobManager manager, AccountManger accountManager)
        {
            _manager = manager;
            _accountManager = accountManager;
        }

        public async Task<Job> GetJob(string id)
        {
            return await _manager.GetJob(id);
        }

        public async Task<IEnumerable<Job>> GetJobs(string type, int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await _manager.GetJobs(type, page * pageSize, pageSize);
        }

        public async Task<IQueryable<Job>> GetJobs(int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await _manager.GetJobs(page * pageSize, pageSize);
        }

        public async Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int page, int pageSize, HttpRequestMessage request)
        {
            var data = await GetJobs(type, page, pageSize);
            var total = await _manager.GetTotalJobCount();

            return new PageEnvelope<Job>(total, page, pageSize, AppConstants.DefaultApiRoute, data, request, new Dictionary<string, object>() { ["type"] = type });
        }

        public Task<Job> PostJob(JobModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateResult> UpdateJobTask(Job job, JobTask selectedTask)
        {
            return await _manager.UpdateJobTask(job._id, job.Tasks.IndexOf(selectedTask), selectedTask);
        }

        public async Task<UpdateResult> UpdateJobTasks(Job job, List<JobTask> tasks)
        {
            return await _manager.UpdateJobTask(job._id, tasks);
        }

        public async Task<bool> ResolveAssetRef(JsonPatchDocument<JobTask> taskPatch)
        {
            var AssetRefReplaceOp = taskPatch.Operations.FirstOrDefault(x => x.op == "replace" && x.path == "/AssetRef");
            if (AssetRefReplaceOp != null && AssetRefReplaceOp.value.GetType()== typeof(string))
            {                
                // INFO: Now we need to actually fetch the asset and get shit done
                var asset = await _accountManager.FindAsByIdAsync<Asset>(AssetRefReplaceOp.value.ToString());
                if (asset == null) return false;
                var assetModel = new AssetModel(asset);
                AssetRefReplaceOp.path = "/Asset";
                AssetRefReplaceOp.value = assetModel as object;
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}