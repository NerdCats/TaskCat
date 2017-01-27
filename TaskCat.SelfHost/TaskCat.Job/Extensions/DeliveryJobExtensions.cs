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
        public Dictionary<string, List<JobTaskExtension>> ExtensionsDictionary { get; private set; }

        public DeliveryJobExtensions()
        {
            ExtensionsDictionary = new Dictionary<string, List<JobTaskExtension>>();
            PopulateExtensions();
        }

        private void PopulateExtensions()
        {
            EnlistClassifiedDeliveryExtensions();
            EnlistDeliveryExtensions();
        }

        private void EnlistDeliveryExtensions()
        {
            var extensions = new List<JobTaskExtension>();

            // Delivery task extension

            /* INFO: Our actionable states are FAILED and RETURNED. 
             * We want to extend functionalities on these two states of course.
             **/
            var actionableState = JobTaskState.FAILED | JobTaskState.RETURNED;

            // Making the condition to an expression so that it can be used in need.
            Expression<Func<JobTask, bool>> conditionExpression =
                task => (actionableState & task.State) == task.State;

            var deliveryJobTaskExtension = new JobTaskExtension(
                OrderTypes.Delivery,
                JobTaskTypes.DELIVERY,
                conditionExpression)
            {
                ExecuteExtension = (job, task) => 
                {
                    // Corner case checks
                    if (job == null)
                        throw new ArgumentNullException(nameof(job));
                    if (task?.Type != JobTaskTypes.DELIVERY)
                        throw new ArgumentNullException(nameof(task));

                    /* Get index of current task. We need to know where the task is at 
                     * Because otherwise we wont be able to inject new Tasks inside.
                     */

                    var index = job.Tasks.IndexOf(task);
                    if (index == -1)
                        throw new ArgumentException(nameof(task));

                    // Casting the task as deliveryTask since this extension is attached to this specific job task type
                    var deliveryTask = task as DeliveryTask;
                    if (deliveryTask == null)
                        throw new ArgumentNullException(nameof(deliveryTask));

                    /* I know a lot of these code looks like boilerplate and replicated. I can consolidate those
                     * but they do hurt readability and that's why I allowed to have a little replicated code here
                     */

                    if (deliveryTask.State == JobTaskState.FAILED)
                    {
                        /* INFO: If the delivery task has reached state FAILED,
                         * it is our job to make sure the next task should retry to send
                         * it again
                         */

                        // Increase Job Attempt
                        job.AttemptCount++;

                        // Create a delivery task that will send the product back since delivery has failed
                        var newDeliveryTask = deliveryTask.GenerateRetryTask();
                        newDeliveryTask.SetPredecessor(deliveryTask);

                        InsertTaskIntoTaskChain(job, index, newDeliveryTask);
                    }
                    else if (deliveryTask.State == JobTaskState.RETURNED)
                    {
                        /* INFO: This means the delivery has ended up in RETURNED state.
                         * Since this is a regular Delivery job, that also means this is possibly a
                         * B2C job. Sending it back to office its only thing we can potentially do 
                         * at least for now
                         */

                        deliveryTask.IsTerminatingTask = false;

                        var newDeliveryTask = deliveryTask.GenerateReturnTask();
                        newDeliveryTask.SetPredecessor(deliveryTask);
                        newDeliveryTask.IsTerminatingTask = true;

                        var propSettings = Settings.Get<ProprietorSettings>();
                        if (propSettings?.Address == null)
                            throw new InvalidOperationException("ProprietorSettings is missing Address");
                        newDeliveryTask.To = propSettings.Address;

                        RemoveTasksAfterIndex(job, index);
                        job.AddTask(deliveryTask);
                    }

                    return job;
                }
            };

            extensions.Add(deliveryJobTaskExtension);
            this.ExtensionsDictionary.Add(OrderTypes.Delivery, extensions);
        }

        // Enterprise Delivery
        private void EnlistClassifiedDeliveryExtensions()
        {
            var extensions = new List<JobTaskExtension>();

            // Delivery task extension

            /* INFO: Our actionable states are FAILED and RETURNED. 
             * We want to extend functionalities on these two states of course.
             **/
            var actionableState = JobTaskState.FAILED | JobTaskState.RETURNED;

            // Making the condition to an expression so that it can be used in need.
            Expression<Func<JobTask, bool>> conditionExpression =
                task => (actionableState & task.State) == task.State;

            /* INFO: This is where we construct our delivery job task extension 
             * for the Classified delivery order and its associated job.
             * We enlisted proper order and job task type for this in the constructor.
             * We wrote the actions to be executed in the ExecuteAction Func
             */
            var deliveryJobTaskExtension = new JobTaskExtension(
                OrderTypes.ClassifiedDelivery,
                JobTaskTypes.DELIVERY,
                conditionExpression)
            {
                ExecuteExtension = (job, task) =>
                {
                    // Corner case checks
                    if (job == null)
                        throw new ArgumentNullException(nameof(job));
                    if (task?.Type != JobTaskTypes.DELIVERY)
                        throw new ArgumentNullException(nameof(task));

                    /* Get index of current task. We need to know where the task is at 
                     * Because otherwise we wont be able to inject new Tasks inside.
                     */

                    var index = job.Tasks.IndexOf(task);
                    if (index == -1)
                        throw new ArgumentException(nameof(task));

                    // Casting the task as deliveryTask since this extension is attached to this specific job task type
                    var deliveryTask = task as DeliveryTask;
                    if (deliveryTask == null)
                        throw new ArgumentNullException(nameof(deliveryTask));


                    if (deliveryTask.State == JobTaskState.FAILED)
                    {
                        /* INFO: If the delivery task has reached state FAILED
                         * that only means the next task should try  
                         * to resend the pacakage again. 
                         * 
                         * That also means that we have to push this new job before 
                         * any task after the current task and increase the job attempt.
                         * */

                        // Increase Job Attempt
                        job.AttemptCount++;

                        // Create a delivery task that will send the product back since delivery has failed
                        var newDeliveryTask = deliveryTask.GenerateRetryTask();
                        newDeliveryTask.SetPredecessor(deliveryTask);

                        InsertTaskIntoTaskChain(job, index, newDeliveryTask);
                    }
                    else if (deliveryTask.State == JobTaskState.RETURNED)
                    {
                        /* This means the job has returned. The target site for the
                         * job has declined the package and now we need to handle this.
                         * There are two ways to handle this. First, we need to check 
                         * if this is already a return task. If it is, the product should
                         * be returned to the nearest office.
                         * 
                         * If this is already not a return job then we should try to return it 
                         * to the proper owner. For Enterprise variant of classified delivery 
                         * the product should go back to the actual owner/provider of the product, 
                         * in this case the enterprise user. 
                         * 
                         * For the default version of the classified delivery this should go 
                         * back to the actual seller of the product.
                         * 
                         * Both of these cases can be handled easily since the addresses are already there
                         * */

                        deliveryTask.IsTerminatingTask = false;

                        var newDeliveryTask = deliveryTask.GenerateReturnTask();
                        newDeliveryTask.SetPredecessor(deliveryTask);
                        newDeliveryTask.IsTerminatingTask = true;


                        if (deliveryTask.Variant == DeliveryTaskVariants.Return)
                        {
                            // Delivery Task is already a return task. 
                            // Lets get it back straight to the HQ

                            // Generating a return task for this and modify the To field to point to our office

                            var propSettings = Settings.Get<ProprietorSettings>();
                            if (propSettings?.Address == null)
                                throw new InvalidOperationException("ProprietorSettings is missing Address");
                            newDeliveryTask.To = propSettings.Address;
                        }

                        /* Since this will be the last entry to the Tasks, we need to 
                         * remove all the tasks that we dont need anymore which are listed 
                         * after this and set this one to be the last task 
                         * */

                        RemoveTasksAfterIndex(job, index);
                        job.AddTask(deliveryTask);
                    }

                    return job;
                }
            };

            extensions.Add(deliveryJobTaskExtension);
            this.ExtensionsDictionary.Add(OrderTypes.ClassifiedDelivery, extensions);
        }

        /// <summary>
        /// Insert a task right after a index in the Tasks list or add in the end
        /// </summary>
        /// <param name="job">Referenced job to be considered with</param>
        /// <param name="index">Index the the new task will be inserted after if possible</param>
        /// <param name="newDeliveryTask">New task to insert</param>
        private static void InsertTaskIntoTaskChain(Data.Entity.Job job, int index, DeliveryTask newDeliveryTask)
        {
            if (index < job.Tasks.Count - 1)
            {
                // this means this task is not a terminating task
                job.Tasks[index + 1].SetPredecessor(newDeliveryTask);
                // Push the job after the delivery task itself.
                job.Tasks.Insert(index, newDeliveryTask);
            }
            else
            {
                // This was the last job task, so just adding the new task will suffice
                job.Tasks.Add(newDeliveryTask);
            }
        }

        /// <summary>
        /// Removes every task after the Tasks list
        /// </summary>
        /// <param name="job">Respective job the tasks have to deleted from</param>
        /// <param name="index">The index after the job should be deleted after</param>
        private static void RemoveTasksAfterIndex(Data.Entity.Job job, int index)
        {
            if (index < job.Tasks.Count - 1)
            {
                job.Tasks.RemoveRange(index + 1, job.Tasks.Count - (index + 1));
            }
        }
    }
}
