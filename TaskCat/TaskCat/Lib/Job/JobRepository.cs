namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Entity;
    using Data.Model.Api;
    using Model.Pagination;
    using System.Net.Http;
    using Constants;
    using Data.Model;
    using MongoDB.Driver;
    using Auth;
    using Data.Model.Identity.Response;
    using Marvin.JsonPatch;
    using System.Web.OData.Query;
    using Data.Model.Query;
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

        public async Task<QueryResult<Job>> GetJobs(ODataQueryOptions<Job> query, int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await _manager.GetJobs(query, page * pageSize, pageSize);
        }

        public async Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int page, int pageSize, HttpRequestMessage request)
        {
            var data = await GetJobs(type, page, pageSize);
            var total = await _manager.GetTotalJobCount();

            return new PageEnvelope<Job>(total, page, pageSize, AppConstants.DefaultApiRoute, data, request, new Dictionary<string, string>() { ["type"] = type });
        }

        public async Task<PageEnvelope<Job>> GetJobsEnveloped(ODataQueryOptions<Job> query, int page, int pageSize, HttpRequestMessage request)
        {
            var result = await GetJobs(query, page, pageSize);
            return new PageEnvelope<Job>(result.Total, page, pageSize, AppConstants.DefaultOdataRoute, result.Result, request, request.GetQueryNameValuePairs().ToDictionary(x => x.Key, y => y.Value));
        }

        public Task<Job> PostJob(JobModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<ReplaceOneResult> UpdateJob(Job job)
        {
            return await _manager.UpdateJob(job);
        }

        public async Task<ReplaceOneResult> UpdateJobWithPatch(string jobId, string taskId,  JsonPatchDocument<JobTask> taskPatch)
        {
            var job = await GetJob(jobId);
            if (job == null) throw new ArgumentException("Invalid Job Id provided");

            var selectedTask = job.Tasks.FirstOrDefault(x => x.id == taskId);
            if (selectedTask == null) throw new ArgumentException("Invalid JobTask Id provided");

            await ResolveAssetRef(taskPatch);

            taskPatch.ApplyTo(selectedTask);
            selectedTask.UpdateTask();

            selectedTask.ModifiedTime = DateTime.UtcNow;
            job.ModifiedTime = selectedTask.ModifiedTime;

            var result = await UpdateJob(job);
            return result;
        }

        public async Task<bool> ResolveAssetRef(JsonPatchDocument<JobTask> taskPatch)
        {
            var AssetRefReplaceOp = taskPatch.Operations.FirstOrDefault(x => x.op == "replace" && x.path == "/AssetRef");
            if (AssetRefReplaceOp != null && AssetRefReplaceOp.value.GetType()== typeof(string))
            {                
                // INFO: Now we need to actually fetch the asset and get shit done
                var asset = await _accountManager.FindAsByIdAsync<Data.Entity.Identity.Asset>(AssetRefReplaceOp.value.ToString());
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