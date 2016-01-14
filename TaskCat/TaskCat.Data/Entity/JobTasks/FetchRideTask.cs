namespace TaskCat.Data.Entity.JobTasks
{
    using Interfaces;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    public class FetchRideTask: JobTask, IFetchable
    {
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public AssetEntity SelectedAsset { get; set; }

        public FetchRideTask() : base("FetchTransit")
        {

        }
    }
}
