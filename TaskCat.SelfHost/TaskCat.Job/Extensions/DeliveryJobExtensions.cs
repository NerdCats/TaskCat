namespace TaskCat.Job.Extensions
{
    using System.Collections.Generic;
    using System;
    using System.Linq.Expressions;
    using Data.Lib.Constants;
    using Data.Model;
    using Data.Model.Order;
    using Data.Model.JobTasks;
    using Its.Configuration;
    using Common.Settings;


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
                    if (task?.Type != JobTaskTypes.DELIVERY)
                        throw new ArgumentNullException(nameof(task));

                    // Get index of current task
                    var index = job.Tasks.IndexOf(task);
                    if (index == -1)
                        throw new ArgumentException(nameof(task));

                    var deliveryTask = task as DeliveryTask;
                    if (deliveryTask == null)
                        throw new ArgumentNullException(nameof(deliveryTask));


                    if (deliveryTask.State == JobTaskState.FAILED)
                    {
                        /* INFO: If the delivery task has reached state FAILED
                         * that only means by default the next task should send 
                         * back the delivery to the actual provider. In this case
                         * the provider is the seller since this is for classified delivery.
                         * That also means that we have to push this new job before 
                         * SecureCashDelivery and increase the job attempt
                         * */

                        // Increase Job Attempt
                        job.AttemptCount++;

                        // Create a delivery task that will send the product back since delivery has failed
                        var newDeliveryTask = deliveryTask.GenerateReturnTask();
                        newDeliveryTask.SetPredecessor(deliveryTask);

                        if (index < job.Tasks.Count - 1)
                        {
                            // this means this task is not a terminating task
                            job.Tasks[index + 1].SetPredecessor(newDeliveryTask);
                            // Push the job after the delivery task itself.
                            job.Tasks.Insert(index, newDeliveryTask);
                        }
                        else
                        {
                            job.Tasks.Add(newDeliveryTask);
                        }
                    }
                    else if (deliveryTask.State == JobTaskState.RETURNED)
                    {
                        /* This means the job has returned. The target site for the
                         * job has declined the package and now we need to handle this.
                         * There are two ways to handle this. First, we need to check 
                         * if this is already a return task. If it is, the product should
                         * be returned to the nearest office.
                         * 
                         * If this is already not a return job then we should do the same we
                         * did for the FAILED state
                         * */

                        // Increase Job Attempt
                        job.AttemptCount++;

                        if (deliveryTask.Variant == DeliveryTaskVariants.Return)
                        {
                            // Delivery Task is already a return task. 
                            // Lets get it back straight to the HQ

                            // Generating a return task
                            var newDeliveryTask = deliveryTask.GenerateReturnTask();
                            var propSettings = Settings.Get<ProprietorSettings>();
                            if (propSettings?.Address == null)
                                throw new InvalidOperationException("ProprietorSettings is missing Address");
                            newDeliveryTask.To = propSettings.Address;
                            newDeliveryTask.SetPredecessor(deliveryTask);
                            
                            /* Since this will be the last entry to the Tasks, we need to 
                             * remove all the tasks that we dont need anymore which are listed 
                             * after this and set this one to be the last task 
                             * */

                            deliveryTask.IsTerminatingTask = false;
                            newDeliveryTask.IsTerminatingTask = true;

                            if (index < job.Tasks.Count - 1)
                            {
                                job.Tasks.RemoveRange(index + 1, job.Tasks.Count - (index + 1));
                            }

                            job.AddTask(deliveryTask);
                        }
                        else
                        {
                            // Delivery Task is not a return task. So, it just has returned.
                            // Generating a return task
                            var newDeliveryTask = deliveryTask.GenerateReturnTask();
                            newDeliveryTask.SetPredecessor(deliveryTask);

                            // Its still the same since the last task will be return to the customer
                            deliveryTask.IsTerminatingTask = false;
                            newDeliveryTask.IsTerminatingTask = true;

                            if (index < job.Tasks.Count - 1)
                            {
                                job.Tasks.RemoveRange(index + 1, job.Tasks.Count - (index + 1));
                            }

                            job.AddTask(deliveryTask);
                        }
                    }

                    return job;
                }
            };

            this.extensions.Add(deliveryJobTaskExtension);
        }
    }
}
