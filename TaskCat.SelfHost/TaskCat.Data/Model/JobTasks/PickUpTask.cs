namespace TaskCat.Data.Model.JobTasks
{
    using Lib.Exceptions;
    using System;
    using Identity.Response;
    using Geocoding;

    public abstract class PickUpTask : JobTask
    {
        public DefaultAddress AssetLocation { get; set; }
        public DefaultAddress PickupLocation { get; set; }

        public PickUpTask(string type, DefaultAddress pickupLocation) : base(type)
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
                    if (fromData.PropertyType != typeof(DefaultAddress))
                        throw new InvalidCastException("Type Verification From Field Failed");

                    var toData = type.GetProperty("To");
                    if (toData.PropertyType != typeof(DefaultAddress))
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
            {
                this.State = JobTaskState.IN_PROGRESS;
                UpdateTask();
            }

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
            // INFO: The dependency for AssetLocation is taken off because right now we
            // dont have locations ensured
            IsReadytoMoveToNextTask = (PickupLocation != null && Asset != null && State == JobTaskState.COMPLETED) ? true : false;
            UpdateStateParams();
        }
    }
}
