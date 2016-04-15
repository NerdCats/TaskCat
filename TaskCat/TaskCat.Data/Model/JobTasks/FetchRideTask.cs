namespace TaskCat.Data.Model.JobTasks
{
    using Lib.Constants;
    using Result;
    using Entity.Identity;
    using Geocoding;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FetchRideTask : AssignAssetTask
    {
        public Asset ProposedRide { get; set; } //FIXME: It will definitely not be a hardburned Asset reference of course

        public FetchRideTask(DefaultAddress from, DefaultAddress to, Asset proposedRide = null) : base(JobTaskTypes.FETCH_RIDE, "Fetching Ride", from, to)
        {
            this.Result = new DefaultAssignAssetTaskResult();
            ProposedRide = proposedRide;
        }
    }

    
}
