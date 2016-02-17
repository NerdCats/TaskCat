namespace TaskCat.Data.Model.JobTasks
{
    using Data.Entity;
    using Lib.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Model;
    using Lib.Constants;
    using Identity.Response;
    public class RidePickUpTask : JobTask
    {
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }

        //FIXME: Im really not sure what Im doing here, this doesnt look right
        private bool _ridePickedUp;
        public bool RidePickedUp
        {
            get { return _ridePickedUp; }
            set
            {
                _ridePickedUp = value;
                if (value)
                {
                    IsReadytoMoveToNextTask = true;
                    UpdateTask();
                }
            }
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
                    throw new JobTaskDependencyException("Error occured on dependency assignment",this, ex);
                }
            }

            Predecessor.JobTaskCompleted += Predecessor_JobTaskCompleted;

        }

        private void Predecessor_JobTaskCompleted(JobTask sender, JobTaskResult jobTaskResult)
        {
            if(this.State==JobTaskStates.PENDING)
                this.State = JobTaskStates.IN_PROGRESS;

            try
            {
                var type = jobTaskResult.ResultType;

                var ride = type.GetProperty("Asset");
                if (ride.PropertyType != typeof(AssetModel))
                    throw new InvalidCastException("Type Verification Asset field failed");

                Asset = ride.GetValue(jobTaskResult, null) as AssetModel;

                // FIXME: This from and to relation should be actually 
                // done based on Assets current location
                
                var fromData = type.GetProperty("From");
                if (fromData.PropertyType != typeof(Location))
                    throw new InvalidCastException("Type Verification From Field Failed");

                FromLocation = fromData.GetValue(jobTaskResult, null) as Location;

                var toData = type.GetProperty("To");
                if (toData.PropertyType != typeof(Location))
                    throw new InvalidCastException("Type Verification To Field Failed");
                ToLocation = toData.GetValue(jobTaskResult, null) as Location; 
            }
            catch (Exception)
            {
                throw;
            }

        }

        public override void UpdateTask()
        {
            MoveToNextState();
        }

    }
}