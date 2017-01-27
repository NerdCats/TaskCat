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
    using System.Runtime.CompilerServices;
    using Utility;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class Job : HRIDEntity, INotifyPropertyChanged
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
            set
            {
                Set(ref _name, value);
            }
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
            set
            {
                Set(ref _user, value);
            }
        }

        private UserModel _jobServedBy;
        public UserModel JobServedBy
        {
            get
            {
                return _jobServedBy;
            }
            set
            {
                Set(ref _jobServedBy, value);
            }
        }

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
                Set(ref state, value);
            }
        }

        public string HRState {
            get
            {
                var lastSignificantTask = this.Tasks.LastOrDefault(x => x.State > JobTaskState.PENDING);

                if (lastSignificantTask != null)
                    return lastSignificantTask.GetHRState();
                else return this.state.GetSimplifiedStateString();
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

        public event PropertyChangedEventHandler PropertyChanged;

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

        private string _paymentMethod;
        public string PaymentMethod
        {
            get
            {
                return _paymentMethod;
            }
            set
            {
                Set(ref _paymentMethod, value);
            }
        }

        private PaymentStatus _paymentStatus;
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentStatus PaymentStatus
        {
            get
            {
                return _paymentStatus;
            }
            set
            {
                Set(ref _paymentStatus, value);
            }
        }

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

        public int AttemptCount { get; set; } = 1;

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
            // Set Job state based on job tasks state
            if (jobTask.State >= JobTaskState.IN_PROGRESS
                && jobTask.State < JobTaskState.COMPLETED
                && State != JobState.IN_PROGRESS)
            {
                State = JobState.IN_PROGRESS;
                this.InitiationTime = this.InitiationTime ?? DateTime.UtcNow;
            }
            else if (jobTask.State == JobTaskState.CANCELLED)
                State = JobState.CANCELLED;

            if (this.tasks.All(x => x.State == JobTaskState.COMPLETED))
            {
                this.CompletionTime = DateTime.UtcNow;
                this.State = JobState.COMPLETED;
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

        // INFO: This is definitely code replication. But I currently don't have a good idea
        // to make it reusable 

        protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
    }
}
