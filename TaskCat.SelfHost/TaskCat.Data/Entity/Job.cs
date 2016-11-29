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
    using Model.Payment;
    using System.ComponentModel;
    using Model.Vendor.ProfitSharing;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class Job : HRIDEntity
    {
        [BsonIgnore]
        [JsonIgnore]
        public bool IsJobTasksEventsHooked = false;

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

        public Dictionary<string, AssetModel> Assets { get; set; }

        private List<JobTask> tasks;
        public List<JobTask> Tasks { get { return tasks; } set { tasks = value; } }

        private JobState state;
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobState State
        {
            get { return state; }
            set
            {
                state = value;
            }
        }

        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public bool ETAFailed
        {
            get
            {
                if (this.Order.ETA.HasValue && this.State == JobState.IN_PROGRESS)
                    return DateTime.UtcNow.Subtract(this.Order.ETA.Value).TotalSeconds > 0;
                else if (this.Order.ETA.HasValue && this.CompletionTime.HasValue)
                    return CompletionTime > this.Order.ETA;
                return false;
            }
        }
        public DateTime? CompletionTime { get; set; }
        public DateTime? InitiationTime { get; set; }
        public TimeSpan? Duration
        {
            get
            {
                if (CompletionTime.HasValue && InitiationTime.HasValue)
                {
                    return CompletionTime.Value.Subtract(InitiationTime.Value);
                }
                else if (InitiationTime.HasValue)
                {
                    return DateTime.UtcNow.Subtract(InitiationTime.Value);
                }
                return null;
            }
        }

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

        public string PaymentMethod { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentStatus PaymentStatus { get; set; }

        public string CancellationReason { get; set; }
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public bool IsJobFreezed
        {
            get
            {
                return IsDeleted || State == JobState.CANCELLED;
            }
        }

        public Vendor Vendor { get; set; }
        public ProfitShareResult ProfitShareResult { get; set; }

        private void _terminalTask_JobTaskCompleted(JobTask sender, JobTaskResult result)
        {
            CompleteJob();
        }

        public void CompleteJob()
        {
            if (!Tasks.All(x => x.State == JobTaskState.COMPLETED))
            {
                throw new NotSupportedException("Setting Job State to COMPLETED when all the job Tasks are not completed");
            }
            State = JobState.COMPLETED;
            CompletionTime = DateTime.UtcNow;
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


        public void EnsureJobTaskChangeEventsRegistered(bool isFetchingJobPayload = false)
        {
            if (!isFetchingJobPayload)
            {
                if (this.Tasks.Any(x => x.State == JobTaskState.COMPLETED))
                    throw new InvalidOperationException("Job Task initialized in COMPLETED state");
            }
            tasks.ForEach(x => x.PropertyChanged += JobTask_PropertyChanged);
            IsJobTasksEventsHooked = true;
        }

        private void JobTask_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is JobTask))
                throw new ArgumentException($"sender is not of type {typeof(JobTask).Name}");

            var task = sender as JobTask;

            this.ModifiedTime = DateTime.UtcNow;
            switch (e.PropertyName)
            {
                case nameof(JobTask.State):
                    SetProperJobState(task);
                    break;
                case nameof(JobTask.AssetRef):
                    if (!Assets.ContainsKey(task.AssetRef))
                        Assets[task.AssetRef] = task.Asset;
                    break;
            }
        }

        private void SetProperJobState(JobTask jobTask)
        {
            if (jobTask.State >= JobTaskState.IN_PROGRESS
                && jobTask.State < JobTaskState.COMPLETED
                && state != JobState.IN_PROGRESS)
            {
                state = JobState.IN_PROGRESS;
                this.InitiationTime = this.InitiationTime ?? DateTime.UtcNow;
            }
            else if (jobTask.State == JobTaskState.CANCELLED)
                state = JobState.CANCELLED;

            if (this.tasks.All(x => x.State == JobTaskState.COMPLETED))
            {
                this.CompletionTime = DateTime.UtcNow;
                this.state = JobState.COMPLETED;
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

        }

        private string GenerateDefaultJobName()
        {
            return string.Format("{0} Job for {1}", this.Order.Type, string.IsNullOrWhiteSpace(User.UserName) ? User.UserId : User.UserName);
        }
    }
}
