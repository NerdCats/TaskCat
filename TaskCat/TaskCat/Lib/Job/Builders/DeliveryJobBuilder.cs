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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DeliveryJobBuilder : JobBuilder
    {
        private IPaymentMethod paymentMethod;
        private DeliveryOrder order;

        public DeliveryJobBuilder(DeliveryOrder order, UserModel userModel, IHRIDService hridService, IPaymentMethod paymentMethod) : base(order, userModel,hridService)
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
            //FIXME: Looks like I can definitely refactor this and work this out
            
            job.Tasks = new List<JobTask>();

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

            job.PaymentMethod = this.paymentMethod;
            job.PaymentStatus = PaymentStatus.Pending;

            job.TerminalTask = deliveryTask;

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
    }
}