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
        
        public JobBuilder(OrderModel order, UserModel userModel, IHRIDService hridService, VendorProfile vendorProfile = null)
        {
            job = new Job(order, hridService.NextId("Job"));
            job.User = userModel;
            job.VendorProfile = vendorProfile;
            job.ProfitShareResult = vendorProfile?.Strategy?.Calculate(order.OrderCart.TotalToPay.Value);
        }

        public JobBuilder(OrderModel order, UserModel userModel, UserModel adminUserModel, IHRIDService hridService, VendorProfile vendorProfile = null) 
            : this(order, userModel, hridService, vendorProfile)
        {
            if (adminUserModel == null)
                throw new ArgumentNullException(nameof(adminUserModel));

            job.JobServedBy = adminUserModel;
        }
    }
}