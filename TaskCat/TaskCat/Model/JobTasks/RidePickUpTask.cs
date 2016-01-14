namespace TaskCat.Model.JobTasks
{
    using Data.Entity.Assets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Model;
    public class RidePickUpTask : JobTask
    {
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public Ride SelectedRide { get; set; }

        public RidePickUpTask() : base("RidePickUp")
        {

        }

        public override bool IsReadyToMoveToNextTask()
        {
            if (FromLocation == null || ToLocation == null || SelectedRide == null)
                return false;
            return true;
        }
    }
}