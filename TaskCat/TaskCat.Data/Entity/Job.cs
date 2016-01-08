namespace TaskCat.Data.Entity
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Job : DbEntity
    {
        // FIXME: Im still not sure how this would be actually done, because
        // We might have to support anonymous requests
        public string User { get; set; }
        public List<JobTask> Tasks { get; set; }
        public JobState State { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> UserLocation { get; set; }
        public DateTime? PreferredDeliveryTime { get; set; }
       
    }
}
