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
            EnlistClassifiedDeliveryExtensions();
        }

        private void EnlistClassifiedDeliveryExtensions()
        {
            // Delivery task extension

            var actionableState = JobTaskState.FAILED | JobTaskState.RETURNED;
            Expression<Func<JobTask, bool>> conditionExpression =
                task => task.IsTerminatingTask && ((actionableState & task.State) == task.State);

            var deliveryJobTaskExtension = new JobTaskExtension(
                OrderTypes.ClassifiedDelivery, 
                JobTaskTypes.DELIVERY,
                conditionExpression)
            {
                ExecuteExtension = (job, task) =>
                {
                    if (job == null)
                        throw new ArgumentNullException(nameof(job));
                    if (task ==null)
                        throw new ArgumentNullException(nameof(task));

                    if (task.State == JobTaskState.FAILED)
                    {
                        /* INFO: If the delivery task has reached state FAILED
                         * that only means by default the next task should send 
                         * back the delivery to the actual provider. In this case
                         * the provider is the seller since this is for classified delivery.
                         * That also means that we have to push this new job before 
                         * SecureCashDelivery and increase the job attempt
                         * */

                        // Increase Job Attempt
                        job.AttemptCount ++;

                        // Create a delivery task that will send the product back since delivery has failed


                        // Push the job after the delivery task itself.
                        var index = job.Tasks.IndexOf(task);
                        if (index == -1)
                            throw new ArgumentException(nameof(task));
                        job.Tasks.Insert(index, null);

                    }
                    return null;
                }
            };

            this.extensions.Add(deliveryJobTaskExtension);


        }
    }
}
