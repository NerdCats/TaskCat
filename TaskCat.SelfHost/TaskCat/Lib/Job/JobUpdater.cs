namespace TaskCat.Lib.Job
{
    using Data.Entity;
    using Data.Model;
    using Updaters;

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