namespace TaskCat.Data.Entity
{
    using Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver.GeoJsonObjectModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class Job : DbEntity
    {
        public string Name { get; set; }

        [BsonIgnoreIfNull]
        public OrderModel Order { get; set; }

        // FIXME: Im still not sure how this would be actually done, because
        // We might have to support anonymous requests
        private string _user = "Anonymous";
        public string User { get { return _user; } set { _user = value; } }

        private string _jobServedBy = "Anonymous";
        public string JobServedBy { get { return _jobServedBy; } set { _jobServedBy = value; } }

        public Dictionary<string, Asset> Assets;

        public List<JobTask> Tasks { get; set; }
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof (StringEnumConverter))]
        public JobState State { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> UserLocation { get; set; }

        [BsonIgnoreIfNull]
        public DateTime? PreferredDeliveryTime { get; set; }

        private JobTask _terminalTask;
        [BsonIgnore]
        [JsonIgnore]
        public JobTask TerminalTask
        {
            get { return _terminalTask; }
            set
            {
                _terminalTask = value;
                _terminalTask.IsTerminatingTask = true;
                _terminalTask.JobTaskCompleted += _terminalTask_JobTaskCompleted;
            }
        }

        private void _terminalTask_JobTaskCompleted(JobTask sender, JobTaskResult result)
        {
            State = JobState.COMPLETED;
        }

        public Job()
        {
            CreateTime = DateTime.UtcNow;
            ModifiedTime = DateTime.UtcNow;
        }

        public Job(string name) : this()
        {
            Name = name;

        }

        public Job(OrderModel order) : this()
        {
            this.Name = order.Name;
            this.Order = order;
        }
    }
}
