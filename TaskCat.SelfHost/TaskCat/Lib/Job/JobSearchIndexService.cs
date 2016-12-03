namespace TaskCat.Lib.Job
{
    using System;
    using Data.Entity;
    using Common.Search;

    public class JobSearchIndexService
    {
        private ISearchContext context;
        private IObservable<Job> jobIndexSource;

        public JobSearchIndexService(IObservable<Job> jobIndexSource, ISearchContext context)
        {
            this.jobIndexSource = jobIndexSource;
            this.context = context;
        }
    }
}
