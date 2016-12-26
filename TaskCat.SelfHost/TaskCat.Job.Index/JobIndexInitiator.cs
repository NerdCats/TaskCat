namespace TaskCat.Job.Index
{
    using Common.Db;
    using Common.Search;
    using System;
    using MongoDB.Driver;
    using Data.Entity;
    using NLog;

    public class JobIndexInitiator : IDisposable
    {
        private IDbContext dbContext;
        private ISearchContext searchContext;
        private IMongoCollection<Job> jobs;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public JobIndexInitiator(IDbContext dbContext, ISearchContext searchContext)
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

        public bool IsInitiationNeeded()
        {
            var coldStorageCount = dbContext.Jobs.Count(x => true);
            var searchContextCount = searchContext.Client.Count<Job>();

            if (!searchContextCount.IsValid)
                throw new InvalidOperationException("Failed to connect to search context", searchContextCount.OriginalException);

            if (coldStorageCount > searchContextCount.Count)
            {
                var indexPercentage = ((double)searchContextCount.Count / (double)coldStorageCount) * 100;
                logger.Info($"{indexPercentage} of jobs are indexed in the system");
                return indexPercentage <= 5;
            }

            return false;
        }

        public void Start()
        {
            var jobCursor = this.jobs.Find(x => true).SortByDescending(x => x.Id).ToCursor();
            logger.Info($"Starting job index initialization");
            while (jobCursor.MoveNext())
            {
                var jobs = jobCursor.Current;
                foreach (var job in jobs)
                {
                    this.searchContext.Client.Index(job, idx=>idx
                    .Index(nameof(Job).ToLowerInvariant())
                    .Type(job.Order.Type));
                }           
            }
            logger.Info($"Job index initialization is done");
        }

        public void Dispose()
        {
            if (dbContext != null)
                this.dbContext.Dispose();
        }
    }
}
