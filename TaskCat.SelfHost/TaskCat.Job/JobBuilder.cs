namespace TaskCat.Job
{
    using Data.Model;
    using Data.Entity;
    using Data.Model.Identity.Response;
    using System;
    using Common.HRID;

    public abstract class JobBuilder
    {
        protected Job job;
        public Job Job { get { return job; } }

        public abstract void BuildJob();
        
        public JobBuilder(OrderModel order, UserModel userModel, IHRIDService hridService, Data.Entity.Vendor vendor = null)
        {
            job = new Job(order, hridService.NextId("Job"));
            job.User = userModel;
            job.Vendor = vendor;
            job.ProfitShareResult = vendor?.Strategy?.Calculate(order.OrderCart.TotalToPay.Value);
        }

        public JobBuilder(OrderModel order, UserModel userModel, UserModel adminUserModel, IHRIDService hridService, Data.Entity.Vendor vendor = null) 
            : this(order, userModel, hridService, vendor)
        {
            if (adminUserModel == null)
                throw new ArgumentNullException(nameof(adminUserModel));

            job.JobServedBy = adminUserModel;
        }
    }
}