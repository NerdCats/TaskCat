namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
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