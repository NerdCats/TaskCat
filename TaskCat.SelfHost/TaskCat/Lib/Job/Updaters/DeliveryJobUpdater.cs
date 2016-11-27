namespace TaskCat.Lib.Job.Updaters
{
    using System;
    using System.Linq;
    using Data.Model;
    using Data.Entity;
    using Data.Model.Order;
    using KellermanSoftware.CompareNetObjects;
    using Data.Lib.Constants;
    using Data.Model.JobTasks;
    using Data.Model.Payment;
    using System.Collections.Generic;
    using Data.Model.Order.Delivery;

    public class DeliveryJobUpdater : JobUpdater
    {
        private List<string> supportedOrderTypes = new List<string>()
        {
            OrderTypes.Delivery,
            OrderTypes.ClassifiedDelivery
        };

        public DeliveryJobUpdater(Job job) : base(job)
        {
        }

        public override void UpdateJob(OrderModel order)
        {
            if (job.State >= JobState.COMPLETED)
            {
                throw new NotSupportedException("Updating order of a completed/cancelled job is not supported");
            }

            if (!supportedOrderTypes.Any(x => x == order.Type))
            {
                throw new NotSupportedException("Invalid Order Type provided");
            }

            if (job.Order.Type != order.Type)
            {
                throw new NotSupportedException("Job and updated order type mismatch");
            }

            if (!string.Equals(job.Order.Variant, order.Variant))
            {
                throw new NotSupportedException("Order variant type changed, order variant type is not updateable");
            }

            // Checking whether the new orders are okay or not
            job.Order.Description = order.Description;
            (job.Order as DeliveryOrder).NoteToDeliveryMan = (order as DeliveryOrder).NoteToDeliveryMan;
            (job.Order as DeliveryOrder).RequiredChangeFor = (order as DeliveryOrder).RequiredChangeFor;
            job.Order.ETA = order.ETA;
            job.Order.ETAMinutes = order.ETAMinutes;

            if(order.Type == OrderTypes.ClassifiedDelivery)
            {
                (job.Order as ClassifiedDeliveryOrder).BuyerInfo = (order as ClassifiedDeliveryOrder).BuyerInfo;
                (job.Order as ClassifiedDeliveryOrder).SellerInfo = (order as ClassifiedDeliveryOrder).SellerInfo;
            }

            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.ComparePrivateFields = true;

            var orderCartComparisonResult = compareLogic.Compare(job.Order.OrderCart, order.OrderCart);

            // Fetching the job task that is currently in progress
            var jobTaskCurrentlyInProgress = job.Tasks.FirstOrDefault(x => x.State == JobTaskState.IN_PROGRESS);
            string firstJobTaskTypeToUpdate = null;

            // Usually this jobTask depicts the state of the JOB itself
            // And this definitely needs refactoring mama
            if (jobTaskCurrentlyInProgress == null && job.State > JobState.ENQUEUED)
                throw new NotSupportedException("No job task is currently in progress, this job is probably close to finished");
            else if (jobTaskCurrentlyInProgress == null && job.State == JobState.ENQUEUED)
                firstJobTaskTypeToUpdate = JobTaskTypes.FETCH_DELIVERYMAN;
            else if (jobTaskCurrentlyInProgress != null)
                firstJobTaskTypeToUpdate = jobTaskCurrentlyInProgress.Type;

            if (firstJobTaskTypeToUpdate == JobTaskTypes.SECURE_DELIVERY)
                throw new NotSupportedException("Cant update order when secure delivery is in progress");

            switch (firstJobTaskTypeToUpdate)
            {
                case JobTaskTypes.FETCH_DELIVERYMAN:
                    FetchDeliveryManTask fetchDeliveryManTask = job.Tasks.First() as FetchDeliveryManTask;
                    fetchDeliveryManTask.From = order.From;
                    fetchDeliveryManTask.To = order.To;
                    fetchDeliveryManTask.State = JobTaskState.PENDING;
                    goto case JobTaskTypes.PACKAGE_PICKUP;
                case JobTaskTypes.PACKAGE_PICKUP:
                    PackagePickUpTask pickUpTask = job.Tasks[1] as PackagePickUpTask;
                    pickUpTask.PickupLocation = order.From;

                    // TODO: need explanation
                    // pickUpTask.State = JobTaskState.PENDING;

                    if (!orderCartComparisonResult.AreEqual)
                    {
                        job.Order.OrderCart = order.OrderCart;
                    }
                    goto case JobTaskTypes.DELIVERY;
                case JobTaskTypes.SECURE_DELIVERY:
                case JobTaskTypes.DELIVERY:
                    DeliveryTask deliveryTask = job.Tasks[2] as DeliveryTask;
                    deliveryTask.From = order.From;
                    deliveryTask.To = order.To;
                    
                    // TODO: need explanation
                    //deliveryTask.State = JobTaskState.PENDING;

                    job.Order.From = order.From;
                    job.Order.To = order.To;

                    if (!orderCartComparisonResult.AreEqual)
                    {
                        job.Order.OrderCart = order.OrderCart;
                    }
                    break;
            }

            job.Name = order.Name;
            job.ModifiedTime = DateTime.UtcNow;

            if (order.PaymentMethod != job.Order.PaymentMethod)
            {
                if (job.PaymentStatus >= PaymentStatus.Authorized && job.PaymentStatus <= PaymentStatus.Refunded)
                    throw new InvalidOperationException($"Current payment method {job.PaymentMethod} is on state {job.PaymentStatus}, Changing payment mehtod is not supported");
                job.Order.PaymentMethod = order.PaymentMethod;
                job.PaymentStatus = PaymentStatus.Pending;
            }
        }
    }
}