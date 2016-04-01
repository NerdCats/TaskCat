namespace TaskCat.Lib.Job
{
    using Data.Model;
    using TaskCat.Data.Entity;
    public abstract class JobBuilder
    {
        protected Job _job;
        public Job Job { get { return _job; } }

        public abstract void BuildTasks();

        public JobBuilder(string name)
        {
            _job = new Job(name);
        }

        public JobBuilder(OrderModel order)
        {
            _job = new Job(order);
        }

    }
}