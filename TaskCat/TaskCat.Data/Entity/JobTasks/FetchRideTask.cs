namespace TaskCat.Data.Entity.JobTasks
{
    using MongoDB.Driver.GeoJsonObjectModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    public class FetchRideTask: JobTask
    {
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public AssetEntity SelectedAsset { get; set; }
        
        public FetchRideTask() : base("FetchRide")
        {

        }

        
    }
}
