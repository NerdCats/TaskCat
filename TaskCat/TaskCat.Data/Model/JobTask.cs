namespace TaskCat.Data.Model
{
    using Entity;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class JobTask
    {
        protected JobTask Successor;

        //FIXME: The result type would definitelty not be string of course
        protected delegate void JobTaskCompletedEventHandler(JobTask sender, JobTaskResult result);
        protected event JobTaskCompletedEventHandler JobTaskCompleted;

        protected delegate void JobTaskStateUpdatedEventHandler(JobTask sender, JobTaskStates updatedState);
        protected event JobTaskStateUpdatedEventHandler JobTaskStateUpdated;

        //FIXME: I still dont know how Im going to implement this!
        //protected delegate void JobTaskUpdatedEventHandler(JobTask sender, string result);
        //protected event JobTaskUpdatedEventHandler JobTaskUpdated;

        public JobTaskResult Result { get; set; }

        public string Type { get; set; }
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

        public bool IsTerminatingTask { get; set; } = true;

        public JobTask()
        {
            
        }

        public JobTask(string type)
        {
            Type = type;
        }

        public virtual void SetSuccessor(JobTask task)
        {
            this.Successor = task;
            IsTerminatingTask = false;
        }

        public virtual void MoveToNextState(JobTaskResult result = null)
        {
            if (State <= JobTaskStates.COMPLETED)
            {
                State++;
                if (JobTaskCompleted != null)
                    JobTaskCompleted(this, result);
            }
            else
            {
                throw new InvalidOperationException(string.Concat("Cant move job state after ", State));
            }

            if (State == JobTaskStates.COMPLETED && result != null)
                NotifyJobTaskCompleted(result);
        }



        protected virtual void NotifyJobTaskCompleted(JobTaskResult result)
        {
            if (result.TaskCompletionTime == null)
                result.TaskCompletionTime = DateTime.UtcNow;

            IsReadytoMoveToNextTask = true;

            //FIXME: the JobTaskResult type has to be initiated
            if (JobTaskCompleted != null)
                JobTaskCompleted(this, result);
        }

    }


    public class JobTaskResult
    {
        public DateTime? TaskCompletionTime { get; set; }
        public object Result { get; set; }
    }
}
