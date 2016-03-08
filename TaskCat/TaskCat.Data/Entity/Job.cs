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
    using Model.Identity.Response;

    public class Job : DbEntity
    {
        [BsonIgnore]
        [JsonIgnore]
        public bool IsAssetEventsHooked = false;

        public string Name { get; set; }

        [BsonIgnoreIfNull]
        public OrderModel Order { get; set; }

        // FIXME: Im still not sure how this would be actually done, because
        // We might have to support anonymous requests
        private string _user = "Anonymous";
        public string User { get { return _user; } set { _user = value; } }

        private string _jobServedBy = "Anonymous";
        public string JobServedBy { get { return _jobServedBy; } set { _jobServedBy = value; } }

        public Dictionary<string, AssetModel> Assets;

        private List<JobTask> tasks;
        public List<JobTask> Tasks { get { return tasks; } set { tasks = value; EnsureTaskAssetEventsAssigned(); } }

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
            this.Assets = new Dictionary<string, AssetModel>();
        }

        public void EnsureTaskAssetEventsAssigned()
        {
            if(this.Tasks!=null && this.Tasks.Count>0)
            {
                foreach (var task in Tasks)
                {
                    task.AssetUpdated += Jtask_AssetUpdated;
                }
                IsAssetEventsHooked = true;
            }         
        }

        public void EnsureAssetModelsPropagated()
        {
            if (this.Tasks != null && this.Tasks.Count > 0)
            {
                foreach (var task in Tasks)
                {
                    if (task.AssetRef != null)
                    {
                        var asset = Assets[task.AssetRef];
                        task.Asset = Assets[task.AssetRef];
                    }
                }
            }
        }

        public void AddTask(JobTask jtask)
        {
            if (this.Tasks == null)
                Tasks = new List<JobTask>();
            this.Tasks.Add(jtask);
            jtask.AssetUpdated += Jtask_AssetUpdated;
        }

        private void Jtask_AssetUpdated(string AssetRef, AssetModel asset)
        {
            if (!Assets.ContainsKey(AssetRef))
                Assets[AssetRef] = asset;
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
