namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Entity;
    public abstract class JobBuilder
    {
        protected JobEntity _job;
        public JobEntity Job { get { return _job; } }

        public abstract void BuildTaks();

        public JobBuilder(string name)
        {
            _job = new JobEntity(name);
        }
    }
}