namespace TaskCat.Data.Entity
{
    using Model;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Model.Identity.Response;
    using System.Linq;

    public class Job : HRIDEntity
    {
        [BsonIgnore]
        [JsonIgnore]
        public bool IsAssetEventsHooked = false;

        private string _name;
        public string Name
        {
            get
            {
                return string.IsNullOrWhiteSpace(_name) ?
                    GenerateDefaultJobName()
                    : _name;
            }
            set { _name = value; }
        }

        [BsonIgnoreIfNull]
        public OrderModel Order { get; set; }

        // FIXME: Im still not sure how this would be actually done, because
        // We might have to support anonymous requests
        private UserModel _user = null;
        public UserModel User
        {
            get
            {
                return _user;
            }
            set { _user = value; }
        }

        private UserModel _jobServedBy;
        public UserModel JobServedBy { get { return _jobServedBy; } set { _jobServedBy = value; } }

        public Dictionary<string, AssetModel> Assets;

        private List<JobTask> tasks;
        public List<JobTask> Tasks { get { return tasks; } set { tasks = value; EnsureTaskAssetEventsAssigned(); } }

        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobState State { get; set; }

        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }

        [BsonIgnoreIfNull]
        public DateTime? PreferredDeliveryTime { get; set; }

        public string InvoiceId { get; set; }

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

        public Job(string name) : this()
        {
            Name = name;

        }

        public Job(OrderModel order, string hrid) : this()
        {
            this.Name = order.Name;
            if (order == null)
                throw new ArgumentNullException("order is provided null");
            this.Order = order;
            if (string.IsNullOrEmpty(hrid))
                throw new ArgumentException("hrid is provided null");
            this.HRID = hrid;
        }

        public void EnsureTaskAssetEventsAssigned()
        {
            if (this.Tasks != null && this.Tasks.Count > 0)
            {
                foreach (var task in Tasks)
                {
                    task.AssetUpdated += Jtask_AssetUpdated;
                }
                IsAssetEventsHooked = true;
            }
        }

        public void EnsureInitialJobState()
        {
            if (Tasks.First().State == JobTaskState.COMPLETED)
                throw new InvalidOperationException("Job Task initialized in COMPLETED state");
            if (Tasks.First().State == JobTaskState.IN_PROGRESS)
                State = JobState.IN_PROGRESS;
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

        private string GenerateDefaultJobName()
        {
            return string.Format("{0} Job for {1}", this.Order.Type, string.IsNullOrWhiteSpace(User.UserName) ? User.UserId : User.UserName);
        }

        private void Jtask_AssetUpdated(string AssetRef, AssetModel asset)
        {
            if (!Assets.ContainsKey(AssetRef))
                Assets[AssetRef] = asset;
        }

        public void SetupDefaultBehaviourForFirstJobTask()
        {
            if (Tasks == null || Tasks.Count == 0)
                throw new InvalidOperationException("Trying to set default behaviour for moving job state to in progress without the task building");
            Tasks.First().JobTaskStateUpdated += Job_FirstJobTaskStateUpdated;
        }

        private void Job_FirstJobTaskStateUpdated(JobTask sender, JobTaskState updatedState)
        {
            if (updatedState > JobTaskState.PENDING && TerminalTask != sender)
                State = JobState.IN_PROGRESS;
            else if (updatedState == JobTaskState.IN_PROGRESS && TerminalTask == sender)
                State = JobState.IN_PROGRESS;
            else if (updatedState == JobTaskState.COMPLETED && TerminalTask == sender)
                State = JobState.COMPLETED;
        }
    }
}
