namespace TaskCat.Data.Model.JobTasks
{
    using Lib.Interfaces;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    using Data.Entity;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Lib.Constants;
    using Identity.Response;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FetchRideTask : JobTask, IFetchable 
    {      
        public Location From { get; set; }
        public Location To { get; set; }
        public Asset ProposedRide { get; set; } //FIXME: It will definitely not be a hardburned Asset reference of course
        
        [BsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public INearestAssetProvider provider { get; set; }

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
    
        public async Task<List<AssetModel>> FetchAvailableAssets()
        {
            var data = await provider.FindAssets(From);
            return data as List<AssetModel>;
        }

        public async Task SelectEligibleAsset()
        {
            Asset = await provider.FindNearestEligibleAssets(From);
        }

        public override void UpdateTask()
        {
            //FIXME: I really should use some attribute here to do this
            //this is just plain ghetto
            IsReadytoMoveToNextTask = (From != null && To != null && AssetRef != null) ? true : false;

            MoveToNextState();
        }

        public override JobTaskResult SetResultToNextState()
        {
            var result = new FetchRideTaskResult();
            result.ResultType = typeof(FetchRideTaskResult);
            if (this.AssetRef == null)
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
