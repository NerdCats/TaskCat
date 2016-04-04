namespace TaskCat.Lib.Job
{
    using Data.Model;
    using System;
    using Data.Entity;
    using System.Linq;

    public abstract class JobBuilder
    {
        protected Job job;
        public Job Job { get { return job; } }

        public abstract void BuildTasks();

        public JobBuilder(string name)
        {
            job = new Job(name);
        }

        public JobBuilder(OrderModel order)
        {
            job = new Job(order);
        }
    }
}