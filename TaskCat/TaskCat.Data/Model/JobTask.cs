namespace TaskCat.Data.Model
{
    using Entity;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Utility;
    using System.ComponentModel.DataAnnotations;
    using Identity.Response;
    public abstract class JobTask
    {
        protected string Name;
        public string id { get; protected set; }

        [JsonIgnore]
        [BsonIgnore]
        protected JobTask Predecessor;

        //FIXME: The result type would definitelty not be string of course
        public delegate void JobTaskCompletedEventHandler(JobTask sender, JobTaskResult result);
        public event JobTaskCompletedEventHandler JobTaskCompleted;

        protected delegate void JobTaskStateUpdatedEventHandler(JobTask sender, JobTaskStates updatedState);
        protected event JobTaskStateUpdatedEventHandler JobTaskStateUpdated;

        public delegate void AssetUpdatedEventHandler(string AssetRef, AssetModel asset);
        public event AssetUpdatedEventHandler AssetUpdated;

        //FIXME: I still dont know how Im going to implement this!
        //protected delegate void JobTaskUpdatedEventHandler(JobTask sender, string result);
        //protected event JobTaskUpdatedEventHandler JobTaskUpdated;

        [BsonIgnore]
        [JsonIgnore]
        public JobTaskResult Result { get; protected set; }

        public string Type { get; set; }

        [BsonIgnore]
        public string JobTaskStateString
        {
            get { return StateStringGenerator.GenerateStateString(State, Name); }
        }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobTaskStates State { get; set; }
        
        private AssetModel asset;
        [BsonIgnore]
        public AssetModel Asset {
            get { return asset; }
            set {
                asset = value;
                if(value!=null)
                    this.AssetRef = asset.Id;
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
                if (value != assetRef)
                {
                    assetRef = value;
                    if (AssetUpdated != null)
                    {
                        if (Asset == null)
                            throw new InvalidOperationException("Invoking Asset Updated event without having actual Asset defined, error");
                        AssetUpdated(assetRef, Asset);
                    }
                }
            }
        }

        [BsonIgnoreIfNull]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ETA { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        [BsonIgnoreIfNull]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        [BsonIgnoreIfNull]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Notes { get; set; }

        public bool IsReadytoMoveToNextTask { get; set; }
        public bool IsDependencySatisfied { get; set; }

        public bool IsStartingTask { get; set; } = true;
        public bool IsTerminatingTask { get; set; } = false;

        public bool ETAFailed { get; set; } = false;

        public JobTask()
        {

        }

        public JobTask(string type, string name) : this()
        {
            id = Guid.NewGuid().ToString();
            Type = type;
            Name = name;

            CreateTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
        }

        public abstract void UpdateTask();
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

        public virtual void MoveToNextState()
        {
            if (State == JobTaskStates.IN_PROGRESS && !IsReadytoMoveToNextTask)
                return;

            if (State <=JobTaskStates.IN_PROGRESS)
            {
                State++;
                if (JobTaskStateUpdated != null)
                    JobTaskStateUpdated(this, State);
            }

            if (State == JobTaskStates.COMPLETED && Result != null && IsReadytoMoveToNextTask)
                NotifyJobTaskCompleted();
            else
                throw new InvalidOperationException("Job is not ready to move to next Task");
                    
        }

        protected virtual void NotifyJobTaskCompleted()
        {
            if (Result.TaskCompletionTime == null)
                Result.TaskCompletionTime = DateTime.UtcNow;

            if (!IsReadytoMoveToNextTask)
                throw new InvalidOperationException("JobTask is not ready to move to next task, yet COMPLETED STATE ACHIEVED");

            //FIXME: the JobTaskResult type has to be initiated
            if (JobTaskCompleted != null)
            {
                Result = SetResultToNextState();
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

    //FIXME: Might need to move this to an Util function
    public static class JobTaskResultExtensions
    {
        public static T Cast<T>(this object input)
        {
            return (T)input;
        }
    }
}
