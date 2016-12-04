namespace TaskCat.Lib.Job
{
    using System;
    using Data.Entity;
    using Common.Search;
    using System.Reactive.Linq;
    using System.Reactive.Concurrency;

    public class JobSearchIndexService
    {
        private ISearchContext context;
        private IObservable<Job> jobIndexSource;

        public JobSearchIndexService(ISearchContext context, IObservable<Job> jobIndexSource)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (jobIndexSource == null)
                throw new ArgumentNullException(nameof(jobIndexSource));

            this.jobIndexSource = jobIndexSource;
            this.context = context;

            this.jobIndexSource
                .Subscribe(OnNext, OnError);
        }

        private void OnNext(Job job)
        {
            // TODO: Just saving it on the database for now
            // Log activity here
            context.Client.Index(job, idx => idx.Index(nameof(Job).ToLowerInvariant()));     
        }

        private void OnError(Exception exception)
        {
            Console.WriteLine(exception);
            // TODO: Log the exception here
        }
    }
}
