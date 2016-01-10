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

    public class FetchRideTask<T> : JobTask, IFetchable<T> where T : Ryde // FIXME: RIDE should be categorized as an asset
    {
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public T SelectedAsset { get; set; }

        public FetchRideTask() : base("Fetching Ride", "FetchRide")
        {

        }

        public List<T> FetchAvailableAssets()
        {
            throw new NotImplementedException();
        }

    }
}
