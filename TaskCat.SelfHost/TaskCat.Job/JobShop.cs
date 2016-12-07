namespace TaskCat.Job
{
    public class JobShop
    {
        public Data.Entity.Job Construct(JobBuilder jobBuilder)
        {
            jobBuilder.BuildJob();
            return jobBuilder.Job;
        }
    }
}