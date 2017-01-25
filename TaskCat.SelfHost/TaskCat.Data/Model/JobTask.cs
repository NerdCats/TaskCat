namespace TaskCat.Data.Model
{
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using Identity.Response;
    using Core;

    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class JobTask : ObservableObject
    {
        protected string Name;
        public string id { get; protected set; }

        [JsonIgnore]
        [BsonIgnore]
        protected JobTask Predecessor;

        //FIXME: The result type would definitelty not be string of course
        public delegate void JobTaskCompletedEventHandler(JobTask sender, JobTaskResult result);
        public event JobTaskCompletedEventHandler JobTaskCompleted;

        [BsonIgnore]
        [JsonIgnore]
        public JobTaskResult Result { get; protected set; }

        public string Type { get; set; }

        private string state;
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string State
        {
            get
            {
                return state;
            }
            set
            {
                Set(ref state, value);           
            }
        }
        
        private AssetModel asset;
        [JsonIgnore]
        [BsonIgnore]
        public AssetModel Asset {
            get { return asset; }
            set {
                asset = value;
                if(value!=null)
                    this.AssetRef = asset.UserId;
            }
        }

        private string assetRef;
        public string AssetRef
        {
            get
            {
                return assetRef;
            }
            set
            {
                Set(ref assetRef, value);
            }
        }

        [BsonIgnoreIfNull]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ETA { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? InitiationTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? CompletionTime { get; set; }
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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Notes { get; set; }

        [JsonIgnore]
        public bool IsReadytoMoveToNextTask { get; set; }
        [JsonIgnore]
        public bool IsDependencySatisfied { get; set; }

        [JsonIgnore]
        public bool IsStartingTask { get; set; } = true;
        [JsonIgnore]
        public bool IsTerminatingTask { get; set; } = false;

        public bool ETAFailed
        {
            get
            {
                if (this.ETA.HasValue && this.State == JobTaskState.IN_PROGRESS)
                    return DateTime.UtcNow.Subtract(this.ETA.Value).TotalSeconds > 0;
                else if (this.ETA.HasValue && this.CompletionTime.HasValue)
                    return CompletionTime > ETA;
                return false;
            }
        }

        public JobTask()
        {
            this.State = JobTaskState.PENDING;
        }

        public JobTask(string name) : this()
        {
            this.Name = name;
        }

        public JobTask(string type, string name) : this()
        {
            id = Guid.NewGuid().ToString();
            Type = type;

            CreateTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            this.Name = name;
        }

        public abstract void UpdateTask();
        public abstract string GetHRState();

        public virtual JobTaskResult SetResultToNextState()
        {
            return this.Result;
        }

        public virtual void SetPredecessor(JobTask task, bool validateDependency = true)
        {
            //FIXME: This is weird, just plain weird
            if(validateDependency)
            {
                if (task.Result == null)
                    throw new ArgumentNullException(nameof(task.Result), "Predecessor Task Result is null, please initialize predecessor Task result in consrtuctor before setting it as a predecessor");
            }
            
            this.Predecessor = task;
            IsStartingTask = false;
           
        }

        public virtual void UpdateStateParams()
        {
            if (State == JobTaskState.PENDING)
                return;

            InitiationTime = InitiationTime ?? DateTime.UtcNow;

            if (IsReadytoMoveToNextTask)
                NotifyJobTaskCompleted();
        }

        protected virtual void NotifyJobTaskCompleted()
        {
            if (!IsReadytoMoveToNextTask)
                throw new InvalidOperationException("JobTask is not ready to move to next task, yet COMPLETED STATE ACHIEVED");
       
            this.CompletionTime = DateTime.UtcNow;
            this.InitiationTime = this.InitiationTime ?? this.CompletionTime;
            State = JobTaskState.COMPLETED;
            //FIXME: the JobTaskResult type has to be initiated
            if (JobTaskCompleted != null)
            {
                Result = SetResultToNextState();
                if (Result != null && Result.TaskCompletionTime == null)
                {
                    Result.TaskCompletionTime = CompletionTime;
                }
                JobTaskCompleted(this, Result);
            }
        }
    }

    //Should be moved to a new class file? No? Okay!

    public abstract class JobTaskResult
    {
        public DateTime? TaskCompletionTime { get; set; }       
        public Type ResultType { get; set; }
    }
}
