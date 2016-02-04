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
    public class FetchRideTask<T> : JobTask, IFetchable<T> where T : AssetEntity // FIXME: RIDE should be categorized as an asset
    {      
        public Location From { get; set; }
        public Location To { get; set; }
        public T SelectedAsset { get; set; }

        [BsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public INearestAssetProvider<T> provider { get; set; }

        public FetchRideTask() : base("FetchRide", "Fetching Ride")
        {
            this.Result = new FetchRideTaskResult();
            State = JobTaskStates.IN_PROGRESS;
        }

        public FetchRideTask(Location from, Location to, T selectedAsset = null) : this()
        {
            From = from;
            To = to;
            selectedAsset = null;
        }
    
        public async Task<List<T>> FetchAvailableAssets()
        {
            var data = await provider.FindAssets(From);
            return data as List<T>;
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
        public AssetEntity Asset { get; set; }

        public FetchRideTaskResult()
        {
            this.ResultType = typeof(FetchRideTaskResult);
        }
    }
}
