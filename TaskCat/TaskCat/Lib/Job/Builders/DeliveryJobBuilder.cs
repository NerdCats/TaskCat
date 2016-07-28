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

            if (order.JobTaskETAPreference?.Count > 0)
                SetupJobTaskETAs(order);
        }

        private void SetupJobTaskETAs(DeliveryOrder order)
        {
            var duplicatePref = order.JobTaskETAPreference.GroupBy(x => x.Type).Where(x => x.Count() > 1).FirstOrDefault();
            if (duplicatePref != null && duplicatePref.Count() > 0)
                throw new NotSupportedException("Duplicate preference for one single jobtask type detected");

            if (order.JobTaskETAPreference?.Count > 0)
            {

                foreach (var task in this.job.Tasks)
                {
                    var pref = order.JobTaskETAPreference.FirstOrDefault(x => x.Type == task.Type);
                    if (pref != null)
                    {
                        task.ETA = pref.ETA.HasValue ? pref.ETA : null;
                    }
                }
            }
        }

        private void JobTask_AssetUpdated(string AssetRef, AssetModel asset)
        {
            //FIXME: Replicating code constantly, need to fix these
            if (!job.Assets.ContainsKey(AssetRef))
                job.Assets[AssetRef] = asset;
        }
    }
}