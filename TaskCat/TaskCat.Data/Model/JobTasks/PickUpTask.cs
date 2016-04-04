namespace TaskCat.Data.Model.JobTasks
{
    using Lib.Exceptions;
    using System;
    using Identity.Response;

    public abstract class PickupTask : JobTask
    {
        public Location AssetLocation { get; set; }
        public Location PickupLocation { get; set; }

        public PickupTask(string type, string name, Location pickupLocation) : base(type, name)
        {
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
                    var fromData = type.GetProperty("From");
                    if (fromData.PropertyType != typeof(Location))
                        throw new InvalidCastException("Type Verification From Field Failed");

                    var toData = type.GetProperty("To");
                    if (toData.PropertyType != typeof(Location))
                        throw new InvalidCastException("Type Verification To Field Failed");

                    var ride = type.GetProperty("Asset");
                    if (ride.PropertyType != typeof(AssetModel))
                        throw new InvalidCastException("Type Verification Asset field failed");

                    IsDependencySatisfied = true;
                }
                catch (Exception ex)
                {
                    throw new JobTaskDependencyException("Error occured on dependency assignment", this, ex);
                }
            }

            Predecessor.JobTaskCompleted += Predecessor_JobTaskCompleted;
        }

        private void Predecessor_JobTaskCompleted(JobTask sender, JobTaskResult jobTaskResult)
        {
            if (this.State == JobTaskState.PENDING)
                this.State = JobTaskState.IN_PROGRESS;

            try
            {
                var type = jobTaskResult.ResultType;

                var ride = type.GetProperty("Asset");
                if (ride.PropertyType != typeof(AssetModel))
                    throw new InvalidCastException("Type Verification Asset field failed");

                Asset = ride.GetValue(jobTaskResult, null) as AssetModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override void UpdateTask()
        {
            IsReadytoMoveToNextTask = (AssetLocation != null && PickupLocation != null && Asset != null) ? true : false;
            MoveToNextState();
        }
    }
}
