namespace TaskCat.Model.JobTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Data.Entity;
    using TaskCat.Data.Model;
    using Data.Model.Identity.Response;
    using Data.Lib.Constants;
    using Data.Lib.Exceptions;
    public class DeliveryTask : JobTask
    {
        public Location From { get; set; }
        public Location To { get; set; }
        public AssetModel Asset { get; set; }


        public DeliveryTask() : base(JobTaskTypes.DELIVERY, "Deliverying Package")
        {

        }

        public DeliveryTask(Location from, Location to) : this()
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
            var fromData = type.GetProperty("From");
            if (fromData.PropertyType != typeof(Location))
                throw new InvalidCastException("Type Verification From Field Failed");

            var toData = type.GetProperty("To");
            if (toData.PropertyType != typeof(Location))
                throw new InvalidCastException("Type Verification To Field Failed");

            var asset = type.GetProperty("Asset");
            if (asset.PropertyType != typeof(AssetModel))
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
                From = fromData.GetValue(jobTaskResult, null) as Location;

                var toData = type.GetProperty("To");               
                To = toData.GetValue(jobTaskResult, null) as Location;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override void UpdateTask()
        {
            IsReadytoMoveToNextTask = (From != null && To != null && Asset != null) ? true : false;
            MoveToNextState();
        }
    }
}