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

    public abstract class JobTask
    {
        [BsonIgnore]
        protected JobTask Predecessor;

        //FIXME: The result type would definitelty not be string of course
        public delegate void JobTaskCompletedEventHandler(JobTask sender, JobTaskResult result);
        public event JobTaskCompletedEventHandler JobTaskCompleted;

        protected delegate void JobTaskStateUpdatedEventHandler(JobTask sender, JobTaskStates updatedState);
        protected event JobTaskStateUpdatedEventHandler JobTaskStateUpdated;

        //FIXME: I still dont know how Im going to implement this!
        //protected delegate void JobTaskUpdatedEventHandler(JobTask sender, string result);
        //protected event JobTaskUpdatedEventHandler JobTaskUpdated;

        public JobTaskResult Result { get; protected set; }

        public string Type { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobTaskStates State { get; set; }

        public string JobStateString { get; set; }
        public AssetEntity AssignedAsset { get; set; }
        public DateTime ETA { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? CompletionTime { get; set; }

        public string Description { get; set; }
        public string Notes { get; set; }

        public bool IsReadytoMoveToNextTask { get; set; }
        public bool IsDependencySatisfied { get; set; }

        public bool IsStartingTask { get; set; } = true;
        public bool IsTerminatingTask { get; set; } = false;

        public bool ETAFailed { get; set; } = false;

        public JobTask()
        {
            
        }

        public JobTask(string type) : this()
        {
            Type = type;
        }

        public abstract void UpdateTask();

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

            if (State < JobTaskStates.IN_PROGRESS)
            {
                State++;
                if (JobTaskStateUpdated != null)
                    JobTaskStateUpdated(this, State);
            }
            else
                throw new InvalidOperationException(string.Concat("Cant move job state after ", State));
            
            if (State == JobTaskStates.COMPLETED && Result != null)
                NotifyJobTaskCompleted();            
        }



        protected virtual void NotifyJobTaskCompleted()
        {
            if (Result.TaskCompletionTime == null)
                Result.TaskCompletionTime = DateTime.UtcNow;

            if (!IsReadytoMoveToNextTask)
                throw new InvalidOperationException("JobTask is not ready to move to next task, yet COMPLETED STATE ACHIEVED");

            //FIXME: the JobTaskResult type has to be initiated
            if (JobTaskCompleted != null)
                JobTaskCompleted(this, Result);
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
