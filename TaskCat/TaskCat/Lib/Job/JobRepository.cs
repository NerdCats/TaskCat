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
    using Order;
    using Order.Process;
    using Data.Model.Order;

    public class JobRepository : IJobRepository
    {
        private IJobManager manager;
        private AccountManager accountManager; // FIXME: When a full fledged assetManager comes up this should be replaced by that

        public JobRepository(IJobManager manager, AccountManager accountManager)
        {
            this.manager = manager;
            this.accountManager = accountManager;
        }

        public async Task<Job> GetJob(string id)
        {
            return await manager.GetJob(id);
        }

        public async Task<IQueryable<Job>> GetJobs()
        {
            return await manager.GetJobs();
        }

        public async Task<Job> GetJobByHrid(string hrid)
        {
            return await manager.GetJobByHRID(hrid);
        }

        public async Task<IEnumerable<Job>> GetJobs(string type, int page, int pageSize)
        {
            if (page < 0)
                throw new ArgumentException("Invalid page index provided");
            return await manager.GetJobs(type, page * pageSize, pageSize);
        }

        public async Task<PageEnvelope<Job>> GetJobsEnveloped(string type, int page, int pageSize, HttpRequestMessage request)
        {
            var data = await GetJobs(type, page, pageSize);
            var total = await manager.GetTotalJobCount();

            return new PageEnvelope<Job>(total, page, pageSize, AppConstants.DefaultApiRoute, data, request, new Dictionary<string, string>() { ["type"] = type });
        }

        public Task<Job> PostJob(JobModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<ReplaceOneResult> UpdateJob(Job job)
        {
            return await manager.UpdateJob(job);
        }

        public async Task<ReplaceOneResult> UpdateOrder(string jobId, OrderModel orderModel)
        {
            var job = await GetJob(jobId);
            if (job.Order.Type != orderModel.Type)
            {
                throw new InvalidOperationException("Updating with a different ordermodel for this job");
            }

            if (job.PaymentMethod != orderModel.PaymentMethod)
            {
                throw new InvalidOperationException("Updating payment method of an exisiting order is not supported");
            }

            // FIXME: Finding a resolver here would help here dude
            switch (orderModel.Type)
            {
                case OrderTypes.Delivery:
                    {
                        var orderCalculationService = new DefaultOrderCalculationService();
                        var serviceChargeCalculationService = new DefaultDeliveryServiceChargeCalculationService();
                        var orderProcessor = new DeliveryOrderProcessor(
                            orderCalculationService,
                            serviceChargeCalculationService);
                        orderProcessor.ProcessOrder(orderModel);
                        break;
                    }
            }

            job.Order = orderModel;

            var result = await UpdateJob(job);
            return result;
        }

        public async Task<ReplaceOneResult> UpdateJobTaskWithPatch(string jobId, string taskId, JsonPatchDocument<JobTask> taskPatch)
        {
            var job = await GetJob(jobId);

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
            if (AssetRefReplaceOp != null && AssetRefReplaceOp.value.GetType() == typeof(string))
            {
                // INFO: Now we need to actually fetch the asset and get shit done
                var asset = await accountManager.FindAsByIdAsync<Data.Entity.Identity.Asset>(AssetRefReplaceOp.value.ToString());
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

        public async Task<ReplaceOneResult> Claim(string jobId, string userId)
        {
            var job = await GetJob(jobId);
            var adminUser = await accountManager.FindByIdAsync(userId);
            var userModel = new UserModel(adminUser);
            job.JobServedBy = userModel;

            return await UpdateJob(job);
        }

        public async Task<ReplaceOneResult> CancelJob(string jobId)
        {
            var job = await GetJob(jobId);
            job.State = JobState.CANCELLED;

            var jobTaskToCancel = job.Tasks.LastOrDefault(x => x.State >= JobTaskState.IN_PROGRESS);

            jobTaskToCancel = jobTaskToCancel ?? job.Tasks.First();

            jobTaskToCancel.State = JobTaskState.CANCELLED;
            var result = await UpdateJob(job);
            return result;
        }
    }
}