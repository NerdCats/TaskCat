namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Entity;
    using Data.Model.Api;
    using System.Net.Http;
    using Constants;
    using Data.Model;
    using MongoDB.Driver;
    using Data.Model.Identity.Response;
    using Marvin.JsonPatch;
    using Order;
    using Order.Process;
    using Data.Model.Order;
    using Data.Model.Operation;
    using Updaters;
    using Common.Model.Pagination;
    using Account.Core;
    using System.Reactive.Subjects;

    public class JobRepository : IJobRepository
    {
        private IJobManager manager;
        private AccountManager accountManager; // FIXME: When a full fledged assetManager comes up this should be replaced by that

        public JobRepository(
            IJobManager manager, 
            AccountManager accountManager, 
            Subject<JobActivity> activitySubject)
        {
            this.manager = manager;
            this.accountManager = accountManager;
        }

        public async Task<Job> GetJob(string id)
        {
            return await manager.GetJob(id);
        }

        public IQueryable<Job> GetJobs()
        {
            return manager.GetJobs();
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

        public async Task<UpdateResult<Job>> UpdateJob(Job job)
        {
            var result = await manager.UpdateJob(job);
            return new UpdateResult<Job>(result.MatchedCount, result.ModifiedCount, job);
        }

        public async Task<UpdateResult<Job>> UpdateOrder(Job job, OrderModel orderModel, string mode)
        {
            if (job.Order.Type != orderModel.Type)
            {
                throw new InvalidOperationException("Updating with a different ordermodel for this job");
            }

            // FIXME: Finding a resolver here would help here dude
            switch (orderModel.Type)
            {
                case OrderTypes.Delivery:
                case OrderTypes.ClassifiedDelivery:
                    {
                        var orderCalculationService = new DefaultOrderCalculationService();
                        var serviceChargeCalculationService = new DefaultDeliveryServiceChargeCalculationService();
                        var orderProcessor = new DeliveryOrderProcessor(
                            orderCalculationService,
                            serviceChargeCalculationService);
                        orderProcessor.ProcessOrder(orderModel);
                        var jobUpdater = new DeliveryJobUpdater(job);
                        jobUpdater.UpdateJob(orderModel, mode);
                        job = jobUpdater.Job;
                        break;
                    }
            }

            var result = await UpdateJob(job);
            return new UpdateResult<Job>(result.MatchedCount, result.ModifiedCount, job);
        }

        public async Task<UpdateResult<Job>> UpdateJobTaskWithPatch(Job job, string taskId, JsonPatchDocument<JobTask> taskPatch)
        {
            if (job.State == JobState.CANCELLED)
            {
                throw new NotSupportedException($"Job {job.Id} is in state {job.State}, restore job for further changes");
            }

            var selectedTask = job.Tasks.FirstOrDefault(x => x.id == taskId);
            if (selectedTask == null) throw new ArgumentException("Invalid JobTask Id provided");

            await ResolveAssetRef(taskPatch, selectedTask);

            taskPatch.ApplyTo(selectedTask);
            selectedTask.UpdateTask();

            selectedTask.ModifiedTime = DateTime.UtcNow;
            job.ModifiedTime = selectedTask.ModifiedTime;

            var result = await UpdateJob(job);

            return new UpdateResult<Job>(result.MatchedCount, result.ModifiedCount, job);
        }

        public async Task<bool> ResolveAssetRef(JsonPatchDocument<JobTask> taskPatch, JobTask jobTask)
        {
            var assetRefReplaceOp = taskPatch.Operations.FirstOrDefault(x => x.op == "replace" && x.path == "/AssetRef");
            if (!(assetRefReplaceOp?.value is string)) return false;
            // INFO: Now we need to actually fetch the asset and get shit done
            if (jobTask.State == JobTaskState.COMPLETED)
                throw new InvalidOperationException($"Updating AssetRef of JobTask {jobTask.id} is not suppported as the task is in {jobTask.State} state");

            var asset = await accountManager.FindAsByIdAsync<Data.Entity.Identity.Asset>(assetRefReplaceOp.value.ToString());
            if (asset == null) return false;
            var assetModel = new AssetModel(asset);
            assetRefReplaceOp.path = "/Asset";
            assetRefReplaceOp.value = assetModel;
            return true;
        }

        public async Task<UpdateResult<Job>> Claim(Job job, string userId)
        {
            var adminUser = await accountManager.FindByIdAsync(userId);
            var userModel = new UserModel(adminUser);
            job.JobServedBy = userModel;

            var result = await UpdateJob(job);
            return new UpdateResult<Job>(result.MatchedCount, result.ModifiedCount, job);
        }

        public async Task<UpdateResult<Job>> CancelJob(Job job, string reason)
        {
            job.State = JobState.CANCELLED;
            job.CancellationReason = reason;

            var jobTaskToCancel = job.Tasks.LastOrDefault(x => x.State >= JobTaskState.IN_PROGRESS);

            jobTaskToCancel = jobTaskToCancel ?? job.Tasks.First();

            jobTaskToCancel.State = JobTaskState.CANCELLED;
            var result = await UpdateJob(job);
            return new UpdateResult<Job>(result.MatchedCount, result.ModifiedCount, job);
        }

        public async Task<UpdateResult<Job>> RestoreJob(Job job)
        {
            if (!job.IsJobFreezed)
                throw new NotSupportedException($" job {job.Id} is not freezed to be restored");

            job.State = JobState.ENQUEUED;
            job.Tasks.ForEach(x => x.State = JobTaskState.PENDING);
            job.CancellationReason = null;

            var result = await UpdateJob(job);
            return new UpdateResult<Job>(result.MatchedCount, result.ModifiedCount, job);
        }
    }
}