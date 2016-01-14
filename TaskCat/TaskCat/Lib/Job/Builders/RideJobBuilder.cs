namespace TaskCat.Lib.Job.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Entity;
    public class RideJobBuilder : JobBuilder
    {
        public override void BuildTaks()
        {
            _job = new JobEntity();
        }


    }
}