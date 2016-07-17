﻿namespace TaskCat.Lib.Job.Builders
{
    using Data.Model;
    using Data.Model.JobTasks;
    using Data.Model.Order;
    using System.Collections.Generic;
    using Data.Model.Identity.Response;
    using HRID;
    using Data.Lib.Payment;
    using Data.Model.Payment;
    using System;
    using Data.Entity;
    using System.Linq;
    using Data.Lib.Constants;
    using KellermanSoftware.CompareNetObjects;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DeliveryJobBuilder : JobBuilder
    {
        private IPaymentMethod paymentMethod;
        private DeliveryOrder order;

        public DeliveryJobBuilder(DeliveryOrder order, UserModel userModel, IHRIDService hridService, IPaymentMethod paymentMethod) : base(order, userModel, hridService)
        {
            this.order = order;
            this.paymentMethod = paymentMethod;
        }

        public DeliveryJobBuilder(DeliveryOrder order, UserModel userModel, UserModel adminUserModel, IHRIDService hridService, IPaymentMethod paymentMethod) : base(order, userModel, adminUserModel, hridService)
        {
            this.order = order;
            this.paymentMethod = paymentMethod;
        }

        public override void BuildJob()
        {
            // FIXME: Looks like I can definitely refactor this and work this out

            job.Tasks = new List<JobTask>();

            // INFO: Fetching to 
            FetchDeliveryManTask fetchDeliveryManTask = new FetchDeliveryManTask(order.From, order.To);
            job.Tasks.Add(fetchDeliveryManTask);
            fetchDeliveryManTask.AssetUpdated += JobTask_AssetUpdated;

            PackagePickUpTask pickUpTask = new PackagePickUpTask(order.From);
            pickUpTask.SetPredecessor(fetchDeliveryManTask);
            job.Tasks.Add(pickUpTask);
            pickUpTask.AssetUpdated += JobTask_AssetUpdated;

            DeliveryTask deliveryTask = new DeliveryTask(order.From, order.To);
            deliveryTask.SetPredecessor(pickUpTask);
            job.Tasks.Add(deliveryTask);
            deliveryTask.AssetUpdated += JobTask_AssetUpdated;

            if (order.Type == OrderTypes.ClassifiedDelivery)
            {
                SecureDeliveryTask secureDeliveryTask = new SecureDeliveryTask(order.To, order.From);
                secureDeliveryTask.SetPredecessor(deliveryTask);
                job.Tasks.Add(secureDeliveryTask);
                secureDeliveryTask.AssetUpdated += JobTask_AssetUpdated;

                job.TerminalTask = secureDeliveryTask;
            }
            else if (order.Type == OrderTypes.Delivery)
            {
                job.TerminalTask = deliveryTask;
            }

            job.PaymentMethod = this.paymentMethod.Key;
            job.PaymentStatus = PaymentStatus.Pending;

            job.EnsureTaskAssetEventsAssigned();
            job.EnsureInitialJobState();

            job.SetupDefaultBehaviourForFirstJobTask();
        }


        private void JobTask_AssetUpdated(string AssetRef, AssetModel asset)
        {
            //FIXME: Replicating code constantly, need to fix these
            if (!job.Assets.ContainsKey(AssetRef))
                job.Assets[AssetRef] = asset;
        }

        public override Job UpdateJob(OrderModel order, Job jobToUpdate)
        {
            if (jobToUpdate.State >= JobState.COMPLETED)
            {
                throw new NotSupportedException("Updating order of a completed/cancelled job is not supported");
            }

            if (order.Type != OrderTypes.Delivery || order.Type != OrderTypes.ClassifiedDelivery)
            {
                throw new NotSupportedException("Invalid Order Type provided");
            }

            if (jobToUpdate.Order.Type != order.Type)
            {
                throw new NotSupportedException("Job and updated order type mismatch");
            }

            if (!string.Equals(jobToUpdate.Order.PayloadType, order.PayloadType))
            {
                throw new NotSupportedException("Order payload type changed, order payload type is not updateable");
            }

            // Checking whether the new orders are okay or not
            jobToUpdate.Order.Description = order.Description;
            jobToUpdate.Order.ETA = order.ETA;
            jobToUpdate.Order.ETAMinutes = order.ETAMinutes;

            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.ComparePrivateFields = true;

            var orderCartComparisonResult = compareLogic.Compare(jobToUpdate.Order.OrderCart, order.OrderCart);

            // Fetching the job task that is currently in progress
            var jobTaskCurrentlyInProgress = jobToUpdate.Tasks.FirstOrDefault(x => x.State == JobTaskState.IN_PROGRESS);

            // Usually this jobTask depicts the state of the JOB itself
            if (jobTaskCurrentlyInProgress == null) throw new NotSupportedException("No job task is currently in progress, this job is close to finish or already finished");

            switch (jobTaskCurrentlyInProgress.Type)
            {
                case JobTaskTypes.FETCH_DELIVERYMAN:
                    FetchDeliveryManTask fetchDeliveryManTask = jobToUpdate.Tasks.First() as FetchDeliveryManTask;
                    fetchDeliveryManTask.From = order.From;
                    fetchDeliveryManTask.To = order.To;
                    fetchDeliveryManTask.State = JobTaskState.PENDING;
                    goto case JobTaskTypes.PACKAGE_PICKUP;
                case JobTaskTypes.PACKAGE_PICKUP:
                    PackagePickUpTask pickUpTask = jobToUpdate.Tasks[1] as PackagePickUpTask;
                    pickUpTask.PickupLocation = order.From;
                    pickUpTask.State = JobTaskState.PENDING;

                    if (!orderCartComparisonResult.AreEqual)
                    {
                        jobToUpdate.Order.OrderCart = order.OrderCart;
                    }
                    goto case JobTaskTypes.DELIVERY;
                case JobTaskTypes.DELIVERY:
                    DeliveryTask deliveryTask = jobToUpdate.Tasks[2] as DeliveryTask;
                    deliveryTask.From = order.From;
                    deliveryTask.To = order.To;
                    deliveryTask.State = JobTaskState.PENDING;

                    jobToUpdate.Order.From = order.From;
                    jobToUpdate.Order.To = order.To;

                    if (order.Type == OrderTypes.ClassifiedDelivery)
                    {
                        goto case JobTaskTypes.SECURE_DELIVERY;
                    }
                    break;
                case JobTaskTypes.SECURE_DELIVERY:
                    throw new NotSupportedException("Cant update order when secure delivery is in progress");
            }

            job.Name = order.Name;
            job.ModifiedTime = DateTime.UtcNow;

            if (order.PaymentMethod != jobToUpdate.Order.PaymentMethod)
            {
                if (jobToUpdate.PaymentStatus >= PaymentStatus.Authorized && jobToUpdate.PaymentStatus <= PaymentStatus.Refunded)
                    throw new InvalidOperationException($"Current payment method {jobToUpdate.PaymentMethod} is on state {jobToUpdate.PaymentStatus}, Changing payment mehtod is not supported");
                jobToUpdate.Order.PaymentMethod = order.PaymentMethod;
                jobToUpdate.PaymentStatus = PaymentStatus.Pending;
            }

            return jobToUpdate;
        }
    }
}