namespace TaskCat.Data.Model.JobTasks
{
    using System;
    using Model;
    using Entity;
    using Lib.Constants;
    using Identity.Response;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FetchRideTask : AssignAssetTask
    {
        public Asset ProposedRide { get; set; } //FIXME: It will definitely not be a hardburned Asset reference of course

        public FetchRideTask(Location from, Location to, Asset proposedRide = null) : base(JobTaskTypes.FETCH_RIDE, "Fetching Ride", from, to)
        {
            this.Result = new FetchRideTaskResult();
            State = JobTaskStates.IN_PROGRESS;
            ProposedRide = proposedRide;
        }

        public override JobTaskResult SetResultToNextState()
        {
            var result = new FetchRideTaskResult();
            result.ResultType = typeof(FetchRideTaskResult);
            if (this.Asset == null)
                throw new InvalidOperationException("Moving to next state when Asset is null.");
            result.Asset = this.Asset;
            result.From = this.From;
            result.TaskCompletionTime = DateTime.UtcNow;
            result.To = this.To;

            return result;
        }
    }

    public class FetchRideTaskResult : JobTaskResult
    {
        public Location From { get; set; }
        public Location To { get; set; }
        //FIXME: If asset is only person oriented we might have much
        //much simpler representation of an asset, up until that FetchRideTaskResult would be a bit complicated
        public AssetModel Asset { get; set; }

        public FetchRideTaskResult()
        {
            this.ResultType = typeof(FetchRideTaskResult);
        }
    }
}
