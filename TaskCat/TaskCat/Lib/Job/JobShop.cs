﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskCat.Lib.Job
{
    public class JobShop
    {
        public void Construct(JobBuilder jobBuilder)
        {
            jobBuilder.BuildTasks();
        }
    }
}