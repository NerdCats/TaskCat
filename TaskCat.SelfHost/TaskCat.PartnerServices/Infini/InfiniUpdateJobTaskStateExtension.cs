using System;
using TaskCat.Data.Entity;
using TaskCat.Data.Lib.Constants;
using TaskCat.Data.Lib.Extension;
using TaskCat.Data.Model;
using TaskCat.Data.Model.JobTasks;
using TaskCat.Data.Model.Order;

namespace TaskCat.PartnerServices.Infini
{
    public class InfiniUpdateJobTaskStateExtension : JobTaskExtension
    {
        private readonly IObserver<JobActivity> activitySubject;

        public InfiniUpdateJobTaskStateExtension(IObserver<JobActivity> activitySubject) 
            : base(
                  OrderTypes.ClassifiedDelivery, 
                  JobTaskTypes.DELIVERY,
                  task => ((JobTaskState.FAILED | JobTaskState.RETURNED) & task.State) == task.State)
        {
            this.ExecuteExtension = ExecuteExtensionInternal;
            this.activitySubject = activitySubject ?? throw new ArgumentNullException(nameof(activitySubject));
        }

        private Job ExecuteExtensionInternal(Job job, JobTask task)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (task?.Type != JobTaskTypes.DELIVERY)
                throw new ArgumentNullException(nameof(task));

            var index = job.Tasks.IndexOf(task);
            if (index == -1)
                throw new ArgumentException(nameof(task));

            // Casting the task as deliveryTask since this extension is attached to this specific job task type
            var deliveryTask = task as DeliveryTask;
            if (deliveryTask == null)
                throw new ArgumentNullException(nameof(deliveryTask));

            var jobTaskUpdateActivity = new JobActivity(
                job, JobActivityOperationNames.Update, $"{nameof(Job.Tasks)}[{index}].{nameof(JobTask.State)}")
            {
                Value = deliveryTask.State
            };

            this.activitySubject.OnNext(jobTaskUpdateActivity);

            return job;
        }
    }
}
