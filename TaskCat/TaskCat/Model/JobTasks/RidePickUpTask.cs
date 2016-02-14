namespace TaskCat.Model.JobTasks
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

    public class RidePickUpTask : JobTask
    {
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public Asset SelectedRide { get; set; }

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

        public RidePickUpTask() : base(JobTaskTypes.RIDE_PICKUP, "Pick Up")
        {
            
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

                    FromLocation = fromData.GetValue(task.Result,null) as Location;

                    var toData = type.GetProperty("To");
                    if (toData.PropertyType != typeof(Location))
                        throw new InvalidCastException("Type Verification To Field Failed");

                    ToLocation = toData.GetValue(task.Result, null) as Location;

                    var rideType = type.GetProperty("Asset");
                    if (rideType.PropertyType != typeof(Asset))
                        throw new InvalidCastException("Type Verification Asset field failed");

                    IsDependencySatisfied = true;
                }
                catch (Exception ex)
                {
                    throw new JobTaskDependencyException("Error occured on dependency assignment",this, ex);
                }
            }

        }

        public override void UpdateTask()
        {
            MoveToNextState();
        }
    }
}