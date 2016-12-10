namespace TaskCat.Job.Index
{
    using Common.Db;
    using Common.Search;
    using System;
    using MongoDB.Driver;
    using Data.Entity;

    public class JobIndexInitiator : IDisposable
    {
        private IDbContext dbContext;
        private ISearchContext searchContext;
        private IMongoCollection<Job> jobs;

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

            if (coldStorageCount > searchContextCount.Count)
            {
                var indexPercentage = ((double)searchContextCount.Count / (double)coldStorageCount) * 100;
                return indexPercentage <= 5;
            }

            return false;
        }

        public void Start()
        {
            var jobCursor = this.jobs.Find(x => true).SortByDescending(x => x.Id).ToCursor();
            while (jobCursor.MoveNext())
            {
                var job = jobCursor.Current;
                this.searchContext.Client.Index(job);
            }
        }

        public void Dispose()
        {
            if (dbContext != null)
                this.dbContext.Dispose();
        }
    }
}
