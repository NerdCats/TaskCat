namespace TaskCat.Job.Builders
{
    using Data.Model;
    using Data.Model.JobTasks;
    using Data.Model.Order;
    using System.Collections.Generic;
    using Data.Model.Identity.Response;
    using Data.Lib.Payment;
    using Data.Model.Payment;
    using System;
    using System.Linq;
    using Data.Model.Vendor.ProfitSharing;
    using Data.Entity;
    using Common.HRID;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DeliveryJobBuilder : JobBuilder
    {
        private IPaymentMethod paymentMethod;
        private DeliveryOrder order;
        private ProfitSharingStrategy profitSharingStrategy;

        public DeliveryJobBuilder(DeliveryOrder order, UserModel userModel, IHRIDService hridService, IPaymentMethod paymentMethod, Vendor vendor = null) 
            : base(order, userModel, hridService, vendor)
        {
            this.order = order;
            this.paymentMethod = paymentMethod;
        }

        public DeliveryJobBuilder(DeliveryOrder order, UserModel userModel, UserModel adminUserModel, IHRIDService hridService, IPaymentMethod paymentMethod, Vendor vendor = null) 
            : base(order, userModel, adminUserModel, hridService, vendor)
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

            PackagePickUpTask pickUpTask = new PackagePickUpTask(order.From);
            pickUpTask.SetPredecessor(fetchDeliveryManTask);
            job.Tasks.Add(pickUpTask);

            DeliveryTask deliveryTask = new DeliveryTask(order.From, order.To);
            deliveryTask.SetPredecessor(pickUpTask);
            job.Tasks.Add(deliveryTask);

            if (order.Type == OrderTypes.ClassifiedDelivery && order.Variant == DeliveryOrderVariants.Default)
            {
                SecureDeliveryTask secureDeliveryTask = new SecureDeliveryTask(order.To, order.From);
                secureDeliveryTask.SetPredecessor(deliveryTask);
                job.Tasks.Add(secureDeliveryTask);

                job.TerminalTask = secureDeliveryTask;
            }
            else if (
                (order.Type == OrderTypes.ClassifiedDelivery && order.Variant == DeliveryOrderVariants.EnterpriseDelivery)
                || order.Type == OrderTypes.Delivery)
            {
                job.TerminalTask = deliveryTask;
            }

            job.PaymentMethod = this.paymentMethod.Key;
            job.PaymentStatus = PaymentStatus.Pending;

            job.EnsureJobTaskChangeEventsRegistered();

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
    }
}