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

        private static void VerifyPropertyTypesFromResult(Type type)
        {
            var fromData = type.GetProperty("From");
            if (fromData.PropertyType != typeof(Location))
                throw new InvalidCastException("Type Verification From Field Failed");

            var toData = type.GetProperty("To");
            if (toData.PropertyType != typeof(Location))
                throw new InvalidCastException("Type Verification To Field Failed");

            var ride = type.GetProperty("Asset");
            if (ride.PropertyType != typeof(AssetModel))
                throw new InvalidCastException("Type Verification Asset field failed");
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
