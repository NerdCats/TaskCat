namespace TaskCat.Model.JobTasks
{
    using Lib.Interfaces;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    using Lib.Asset;
    using Data.Entity;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Lib.Constants;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FetchRideTask : JobTask, IFetchable 
    {      
        public Location From { get; set; }
        public Location To { get; set; }
        public Asset SelectedAsset { get; set; }

        [BsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public INearestAssetProvider<Asset> provider { get; set; }

        public FetchRideTask() : base(JobTaskTypes.FETCH_RIDE, "Fetching Ride")
        {
            this.Result = new FetchRideTaskResult();
            State = JobTaskStates.IN_PROGRESS;
        }

        public FetchRideTask(Location from, Location to, Asset selectedAsset = null) : this()
        {
            From = from;
            To = to;
            selectedAsset = null;
        }
    
        public async Task<List<Asset>> FetchAvailableAssets()
        {
            var data = await provider.FindAssets(From);
            return data as List<Asset>;
        }

        public async Task SelectEligibleAsset()
        {
            SelectedAsset = await provider.FindNearestEligibleAssets(From);
        }

        public override void UpdateTask()
        {
            //FIXME: I really should use some attribute here to do this
            //this is just plain ghetto
            IsReadytoMoveToNextTask = (From != null && To != null && SelectedAsset != null) ? true : false;

            MoveToNextState();
        }

        
    }

    public class FetchRideTaskResult : JobTaskResult
    {
        public Location From { get; set; }
        public Location To { get; set; }
        //FIXME: If asset is only person oriented we might have much
        //much simpler representation of an asset, up until that FetchRideTaskResult would be a bit complicated
        public Asset Asset { get; set; }

        public FetchRideTaskResult()
        {
            this.ResultType = typeof(FetchRideTaskResult);
        }
    }
}
