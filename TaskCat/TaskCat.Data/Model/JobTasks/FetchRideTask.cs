namespace TaskCat.Data.Model.JobTasks
{
    using Model;
    using Entity;
    using Lib.Constants;
    using Result;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FetchRideTask : AssignAssetTask
    {
        public Asset ProposedRide { get; set; } //FIXME: It will definitely not be a hardburned Asset reference of course

        public FetchRideTask(Location from, Location to, Asset proposedRide = null) : base(JobTaskTypes.FETCH_RIDE, "Fetching Ride", from, to)
        {
            this.Result = new DefaultAssignAssetTaskResult();
            State = JobTaskState.IN_PROGRESS;
            ProposedRide = proposedRide;
        }
    }

    
}
