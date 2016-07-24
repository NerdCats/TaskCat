namespace TaskCat.Data.Model.JobTasks
{
    using System;
    using Model;
    using Identity.Response;
    using Lib.Constants;
    using Lib.Exceptions;
    using Geocoding;
    using Result;

    public class DeliveryTask : JobTask
    {
        public DefaultAddress From { get; set; }
        public DefaultAddress To { get; set; }

        public DeliveryTask(DefaultAddress from, DefaultAddress to) :
            base(JobTaskTypes.DELIVERY, "Deliverying Package")
        {
            this.From = from;
            this.To = to;
            this.Result = new AssetTaskResult();
        }

        protected DeliveryTask(DefaultAddress from, DefaultAddress to, string type, string name) : 
            base(type, name)
        {
            if (type != JobTaskTypes.SECURE_DELIVERY)
                throw new NotSupportedException($"{type} is not supported as a JobTaskType under Delivery JobTask");
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
            if (this.State == JobTaskState.PENDING)
                this.State = JobTaskState.IN_PROGRESS;

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