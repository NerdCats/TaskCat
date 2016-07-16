namespace TaskCat.Lib.Job.Builders
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

        public override void UpdateJob(OrderModel order, Job job)
        {
            if(job.State == JobState.COMPLETED)
            {
                throw new NotSupportedException("Updating order of a completed job is not supported");
            }

            if (order.Type != OrderTypes.Delivery || order.Type != OrderTypes.ClassifiedDelivery)
            {
                throw new NotSupportedException("Invalid Order Type provided");
            }

            if (job.Order.Type != order.Type)
            {
                throw new NotSupportedException("Job and updated order type mismatch");
            }

            job.Order = order;

            FetchDeliveryManTask fetchDeliveryManTask = job.Tasks.First() as FetchDeliveryManTask;
            fetchDeliveryManTask.From = order.From;
            fetchDeliveryManTask.To = order.To;
            fetchDeliveryManTask.State = JobTaskState.PENDING;

            PackagePickUpTask pickUpTask = job.Tasks[1] as PackagePickUpTask;
            pickUpTask.PickupLocation = order.From;
            pickUpTask.State = JobTaskState.PENDING;

            DeliveryTask deliveryTask = job.Tasks[2] as DeliveryTask;
            deliveryTask.From = order.From;
            deliveryTask.To = order.To;
            deliveryTask.State = JobTaskState.PENDING;

            if (order.Type == OrderTypes.ClassifiedDelivery)
            {
                SecureDeliveryTask secureDeliveryTask = job.Tasks.Last() as SecureDeliveryTask;
                secureDeliveryTask.From = order.To;
                secureDeliveryTask.To = order.From;
                secureDeliveryTask.State = JobTaskState.PENDING;
            }
        }
    }
}