namespace TaskCat.Job.Extensions
{
    using System.Collections.Generic;
    using System;
    using System.Linq.Expressions;
    using Data.Lib.Constants;
    using Data.Model;
    using Data.Model.Order;

    public class DeliveryJobExtensions
    {
        private List<JobTaskExtension> extensions;
        public List<JobTaskExtension> Extensions() => extensions;

        public DeliveryJobExtensions()
        {
            extensions = new List<JobTaskExtension>();
            PopulateExtensions();
        }

        private void PopulateExtensions()
        {
            EnlistDeliveryTaskExtension();
        }

        private void EnlistDeliveryTaskExtension()
        {
            var actionableState = JobTaskState.FAILED | JobTaskState.RETURNED;
            Expression<Func<JobTask, bool>> conditionExpression =
                task => task.IsTerminatingTask && ((actionableState & task.State) > 0);

            var deliveryJobTaskExtension = new JobTaskExtension(
                OrderTypes.ClassifiedDelivery, 
                JobTaskTypes.DELIVERY,
                conditionExpression)
            {
                ExecuteExtension = job =>
                {
                    if (job == null)
                        throw new ArgumentNullException(nameof(job));



                    return null;
                }
            };

            this.extensions.Add(deliveryJobTaskExtension);
        }
    }
}
