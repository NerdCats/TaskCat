namespace TaskCat.Lib.Job
{
    using Data.Model;
    using System;
    using Data.Entity;
    using System.Linq;
    using Data.Model.Identity.Response;
    public abstract class JobBuilder
    {
        protected Job job;
        public Job Job { get { return job; } }

        public abstract void BuildTasks();

        public JobBuilder(string name)
        {
            job = new Job(name);
        }

        public JobBuilder(OrderModel order, UserModel userModel)
        {
            job = new Job(order);
            job.User = userModel;
        }
    }
}