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
    using Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class Job : DbEntity
    {
        public string Name { get; set; }
        // FIXME: Im still not sure how this would be actually done, because
        // We might have to support anonymous requests
        private string _user = "Anonymous";
        public string User { get { return _user; } set { _user = value; } }
        public List<JobTask> Tasks { get; set; }
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof (StringEnumConverter))]
        public JobState State { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> UserLocation { get; set; }
        public DateTime? PreferredDeliveryTime { get; set; }

        public Job()
        {
            CreateTime = DateTime.UtcNow;
            ModifiedTime = DateTime.UtcNow;
        }
    }
}
