namespace TaskCat.Data.Model.JobTasks
{
    using TaskCat.Data.Model;
    using Lib.Constants;
    public class RidePickUpTask : PickupTask
    {
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

        public RidePickUpTask(Location pickupLocation) :base(JobTaskTypes.RIDE_PICKUP, "Picking up", pickupLocation)
        {
            this.PickupLocation = pickupLocation;
        }

        // FIXME: SetResultToNextState and Proper JobTaskResult implementation is missing
        // Need to be very careful about it
    }
}