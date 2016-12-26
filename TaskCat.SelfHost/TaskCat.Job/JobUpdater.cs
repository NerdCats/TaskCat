namespace TaskCat.Job
{
    using Data.Entity;
    using Data.Model;

    public abstract class JobUpdater
    {
        protected Job job;
        public Job Job { get { return job; } }

        public JobUpdater(Job job)
        {
            this.job = job;
        }

        public abstract void UpdateJob(OrderModel order, string mode);
    }
}