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

    public class FetchRideTask<T> : JobTask, IFetchable<T> where T : Ride // FIXME: RIDE should be categorized as an asset
    {
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public T SelectedAsset { get; set; }


        public FetchRideTask() : base("FetchRide")
        {
        }

        public FetchRideTask(Location from, Location to, T selectedAsset = null) : this()
        {
            FromLocation = from;
            ToLocation = to;
            selectedAsset = null;
        }

        public async Task<List<T>> FetchAvailableAssets(INearestAssetProvider<T> provider)
        {
            var data = await provider.FindAssets(FromLocation);
            return data as List<T>;
        }

        public override bool IsReadyToMoveToNextTask()
        {
            if (FromLocation == null || ToLocation == null || SelectedAsset == null)
                return false;
            return true;

        }
    }
}
