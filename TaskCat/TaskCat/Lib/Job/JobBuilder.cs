namespace TaskCat.Lib.Job
{
    using Data.Model;
    using Data.Entity;
    using Data.Model.Identity.Response;
    using HRID;
    using System;
    using Data.Model.Vendor.ProfitSharing;

    public abstract class JobBuilder
    {
        protected Job job;
        public Job Job { get { return job; } }

        public abstract void BuildJob();
        
        public JobBuilder(OrderModel order, UserModel userModel, IHRIDService hridService, ProfitSharingStrategy profitSharingStrategy)
        {
            job = new Job(order, hridService.NextId("Job"));
            job.User = userModel;
            job.ProfitSharingStrategy = profitSharingStrategy;
            job.ProfitShareResult = profitSharingStrategy.Calculate(order.OrderCart.TotalToPay.Value);
        }

        public JobBuilder(OrderModel order, UserModel userModel, UserModel adminUserModel, IHRIDService hridService, ProfitSharingStrategy profitSharingStrategy) 
            : this(order, userModel, hridService, profitSharingStrategy)
        {
            if (adminUserModel == null)
                throw new ArgumentNullException(nameof(adminUserModel));

            job.JobServedBy = adminUserModel;
        }
    }
}