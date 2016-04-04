namespace TaskCat.Model.JobTasks
{
    using System;
    using Data.Model;
    using Data.Model.Identity.Response;
    using Data.Lib.Constants;
    using Data.Lib.Exceptions;

    public class DeliveryTask : JobTask
    {
        public Location From { get; set; }
        public Location To { get; set; }

        public DeliveryTask(Location from, Location to) : base(JobTaskTypes.DELIVERY, "Deliverying Package")
        {
            this.From = from;
            this.To = to;
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
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override void UpdateTask()
        {
            IsReadytoMoveToNextTask = (To != null && Asset != null) ? true : false;
            MoveToNextState();
        }
    }
}