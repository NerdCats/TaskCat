namespace TaskCat.Data.Model.JobTasks
{
    using System;
    using Model;
    using Lib.Constants;
    using Geocoding;
    using Result;

    public class PackagePickUpTask : PickUpTask
    {
        public PackagePickUpTask()
        {
            this.Name = "Package pick up";
        }

        public PackagePickUpTask(DefaultAddress pickupLocation) : base(JobTaskTypes.PACKAGE_PICKUP, pickupLocation)
        {
            this.Name = "Package pick up";
            this.Result = new AssetTaskResult();
        }

        public override JobTaskResult SetResultToNextState()
        {
            var result = new AssetTaskResult();
            result.ResultType = typeof(AssetTaskResult);
            if (this.Asset == null)
                throw new InvalidOperationException("Moving to next state when Asset is null");
            result.Asset = this.Asset;                        
            result.TaskCompletionTime = DateTime.UtcNow;
            return result;
        }
    }
}
