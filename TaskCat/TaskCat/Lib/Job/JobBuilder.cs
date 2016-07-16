namespace TaskCat.Lib.Job
{
    using Data.Model;
    using Data.Entity;
    using Data.Model.Identity.Response;
    using HRID;

    public abstract class JobBuilder
    {
        protected Job job;
        public Job Job { get { return job; } }

        public abstract void BuildJob();
        public abstract void UpdateJob(OrderModel order, Job job);

        public JobBuilder(OrderModel order, UserModel userModel, IHRIDService hridService)
        {
            job = new Job(order, hridService.NextId("Job"));
            job.User = userModel;
        }

        public JobBuilder(OrderModel order, UserModel userModel, UserModel adminUserModel, IHRIDService hridService) : this(order, userModel, hridService)
        {
            job.JobServedBy = adminUserModel;
        }
    }
}