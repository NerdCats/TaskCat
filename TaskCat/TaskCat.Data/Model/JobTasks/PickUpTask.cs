namespace TaskCat.Data.Model.JobTasks
{
    using System;
    using Data.Model;
    using Data.Model.Identity.Response;
    using Data.Lib.Constants;
    using Data.Lib.Exceptions;

    public class PackagePickUpTask : JobTask
    {
        public Location AssetLocation { get; set; }
        public Location PickupLocation { get; set; }

        public PackagePickUpTask(Location pickupLocation) : base(JobTaskTypes.PACKAGE_PICKUP, "Picking up Package")
        {
            this.Result = new PickUpTaskResult();
            this.PickupLocation = pickupLocation;
        }

        public override void SetPredecessor(JobTask task, bool validateDependency = true)
        {
            base.SetPredecessor(task, validateDependency);
            if (validateDependency)
            {
                var type = task.Result.ResultType;
                //FIXME: All of these has to be cached and refactored
                try
                {
                    VerifyPropertyTypesFromResult(type);
                    IsDependencySatisfied = true;
                }
                catch (Exception ex)
                {
                    throw new JobTaskDependencyException("Error occured on dependency assignment", this, ex);
                }
            }
            Predecessor.JobTaskCompleted += Predecessor_JobTaskCompleted;
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

        private void Predecessor_JobTaskCompleted(JobTask sender, JobTaskResult jobTaskResult)
        {
            if (this.State == JobTaskStates.PENDING)
                this.State = JobTaskStates.IN_PROGRESS;
            try
            {
                var type = jobTaskResult.ResultType;

                VerifyPropertyTypesFromResult(type);

                var asset = type.GetProperty("Asset");
                Asset = asset.GetValue(jobTaskResult, null) as AssetModel;

                var fromData = type.GetProperty("From");
                PickupLocation = fromData.GetValue(jobTaskResult, null) as Location;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override void UpdateTask()
        {
            IsReadytoMoveToNextTask = (PickupLocation != null && Asset != null) ? true : false;
            MoveToNextState();
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
