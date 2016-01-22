using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskCat.Data.Entity;

namespace TaskCat.Lib.Job
{
    public class JobShop
    {
        public JobEntity Construct(JobBuilder jobBuilder)
        {
            jobBuilder.BuildTasks();
            return jobBuilder.Job;
        }
    }
}