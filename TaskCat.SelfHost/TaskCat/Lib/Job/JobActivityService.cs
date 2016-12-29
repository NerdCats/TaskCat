namespace TaskCat.Lib.Job
{
    using Common.Db;
    using Data.Entity;
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    public class JobActivityService
    {
        private IDbContext dbcontext;
        private IObservable<JobActivity> jobActivitySource;

        public JobActivityService(IDbContext dbcontext, IObservable<JobActivity> jobActivitySource)
        {
            if (dbcontext == null)
                throw new ArgumentNullException(nameof(dbcontext));
            if (jobActivitySource == null)
                throw new ArgumentNullException(nameof(jobActivitySource));

            this.dbcontext = dbcontext;
            this.jobActivitySource = jobActivitySource;

            this.jobActivitySource
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Subscribe(OnNext, OnError);          
        }

        private void OnNext(JobActivity activity)
        {
            // TODO: Just saving it on the database for now
            // Log activity here
            dbcontext.JobActivityCollection.InsertOne(activity);
        }

        private void OnError(Exception exception)
        {
            Console.WriteLine(exception);
            // TODO: Log the exception here and we might need to redo things here
        }
    }
}
