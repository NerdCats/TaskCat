namespace TaskCat.Data.Model.JobTasks
{
    using System;
    using Data.Model;
    using Data.Model.Identity.Response;
    using Data.Lib.Constants;

    public class PackagePickUpTask : PickupTask
    {
        public PackagePickUpTask(Location pickupLocation) : base(JobTaskTypes.PACKAGE_PICKUP, "Picking up Package", pickupLocation)
        {
            this.Result = new PickUpTaskResult();
        }

        public override JobTaskResult SetResultToNextState()
        {
            var result = new PickUpTaskResult();
            result.ResultType = typeof(PickUpTaskResult);
            if (this.Asset == null)
                throw new InvalidOperationException("Moving to next state when Asset is null");
            result.Asset = this.Asset;                        
            result.TaskCompletionTime = DateTime.UtcNow;
            return result;
        }

        public class PickUpTaskResult : JobTaskResult
        {
            public AssetModel Asset { get; set; }
            public PickUpTaskResult()
            {
                this.ResultType = typeof(PickUpTaskResult);
            }
        }
    }
}
