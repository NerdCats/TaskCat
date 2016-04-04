namespace TaskCat.Data.Model.JobTasks
{
    using System;
    using TaskCat.Data.Model;
    using Data.Entity;
    using Lib.Constants;
    using Identity.Response;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FetchRideTask : JobTask
    {      
        public Location From { get; set; }
        public Location To { get; set; }
        public Asset ProposedRide { get; set; } //FIXME: It will definitely not be a hardburned Asset reference of course

        public FetchRideTask() : base(JobTaskTypes.FETCH_RIDE, "Fetching Ride")
        {
            this.Result = new FetchRideTaskResult();
            State = JobTaskStates.IN_PROGRESS;
        }

        public FetchRideTask(Location from, Location to, Asset selectedAsset = null) : this()
        {
            From = from;
            To = to;
            ProposedRide = null;
        }
           
        public override void UpdateTask()
        {
            //FIXME: I really should use some attribute here to do this
            //this is just plain ghetto
            IsReadytoMoveToNextTask = (From != null && To != null && Asset != null) ? true : false;
            
            MoveToNextState();
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
