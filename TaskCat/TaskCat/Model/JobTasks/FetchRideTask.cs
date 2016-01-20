namespace TaskCat.Model.JobTasks
{
    using Lib.Interfaces;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Data.Entity.Assets;
    using TaskCat.Data.Model;
    using Lib.Asset;
    using Data.Entity;
    public class FetchRideTask<T> : JobTask, IFetchable<T> where T : Ride // FIXME: RIDE should be categorized as an asset
    {
        public Location From { get; set; }
        public Location To { get; set; }
        public T SelectedAsset { get; set; }
        public INearestAssetProvider<T> provider { get; set; }

        public FetchRideTask() : base("FetchRide")
        {
            Predecessor.JobTaskCompleted += Predecessor_JobTaskCompleted;
            this.Result = new FetchRideTaskResult();
            
        }

        public FetchRideTask(Location from, Location to, T selectedAsset = null) : this()
        {
            From = from;
            To = to;
            selectedAsset = null;
        }

        private void Predecessor_JobTaskCompleted(JobTask sender, JobTaskResult result)
        {
            //var payloadType = result.ResultType;
            //var payload = Convert.ChangeType(result, payloadType) as payloadType;
        }
    
        public async Task<List<T>> FetchAvailableAssets()
        {
            var data = await provider.FindAssets(From);
            return data as List<T>;
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
