namespace TaskCat.Lib.Job
{
    using TaskCat.Data.Entity;

    public class JobShop
    {
        public Job Construct(JobBuilder jobBuilder)
        {
            jobBuilder.BuildTasks();
            return jobBuilder.Job;
        }
    }
}