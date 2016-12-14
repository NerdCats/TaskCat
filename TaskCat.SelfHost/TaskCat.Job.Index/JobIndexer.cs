namespace TaskCat.Job.Index
{
    using Common.Db;
    using Common.Search;
    using System;
    using MongoDB.Driver;
    using Data.Entity;
    using System.Threading;
    using NLog;

    public class JobIndexer : IDisposable
    {
        private IDbContext dbContext;
        private IMongoCollection<Job> jobs;
        private ISearchContext searchContext;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public JobIndexer(IDbContext dbContext, ISearchContext searchContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));
            if (searchContext == null)
                throw new ArgumentNullException(nameof(searchContext));
            if (dbContext.Jobs == null)
                throw new ArgumentNullException(nameof(dbContext.Jobs));

            this.dbContext = dbContext;
            this.searchContext = searchContext;
            this.jobs = dbContext.Jobs;
        }

        public void Dispose()
        {
            if (dbContext != null)
                this.dbContext.Dispose();
        }

        public void Start()
        {
            while (true)
            {
                var dateTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(2));
                var jobOrdered = this.jobs.Find(x => x.ModifiedTime.Value > dateTime).SortByDescending(x => x.ModifiedTime).ToCursor();

                while (jobOrdered.MoveNext())
                {
                    var jobs = jobOrdered.Current;
                    foreach (var job in jobs)
                    {
                        var searchContextJob = searchContext.Client.Get<Job>(job.Id);
                        if (searchContextJob != null)
                        {
                            if (job.ModifiedTime > searchContextJob.Source.ModifiedTime)
                            {
                                logger.Info($"Indexing {job.Id}");
                                this.searchContext.Client.Index(job, idx => idx
                                    .Index(nameof(Job).ToLowerInvariant())
                                    .Type(job.Order.Type));
                            }
                        }
                    }
                }
                Thread.Sleep(TimeSpan.FromHours(1.5));
            }
        }
    }
}
