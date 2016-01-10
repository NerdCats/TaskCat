namespace TaskCat.Model.JobTasks
{
    using Data.Interfaces;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Data.Entity.Assets;
    using TaskCat.Data.Model;
    using Lib.Asset;

    public class FetchRideTask<T> : JobTask, IFetchable<T> where T : Ryde // FIXME: RIDE should be categorized as an asset
    {
        private INearestAssetProvider<Ryde> _rydeProvider;

        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public T SelectedAsset { get; set; }


        public FetchRideTask(INearestAssetProvider<Ryde> rydeProvider) : base("Fetching Ride", "FetchRide")
        {
            _rydeProvider = rydeProvider;
        }

        public async Task<List<T>> FetchAvailableAssets()
        {
            var data = await _rydeProvider.FindAssets(null);
            return data as List<T>;
        }
    }
}
