namespace TaskCat.Job.Index
{
    using System;
    using Dichotomy;
    using Its.Configuration;
    using System.Diagnostics;
    using Common.Db;
    using Common.Search;

    public class TaskCatJobIndexService : IDichotomyService
    {
        private JobIndexer jobIndexer;
        private JobIndexInitiator jobInitiatiator;

        public TaskCatJobIndexService()
        {
#if DEBUG
            Settings.Precedence = new[] { "local", "production" };
#else
            Settings.Precedence = new[] { "production", "local" };
#endif
        }

        public void Start()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Starting TaskCat Job Indexing Service");
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;

            var dbContext = new DbContext();
            var searchContext = new SearchContext();

            this.jobInitiatiator = new JobIndexInitiator(dbContext, searchContext);
            this.jobIndexer = new JobIndexer(dbContext, searchContext);

            watch.Stop();
            Console.WriteLine($"Started TaskCat Job Indexing Servicein {watch.Elapsed.TotalSeconds} seconds");
            Console.ForegroundColor = oldColor;
        }

        public void Stop()
        {
            Console.WriteLine("Stopping TaskCat Job Indexing Service Service");
        }
        public void Dispose()
        {
            if (this.jobInitiatiator != null)
                this.jobInitiatiator.Dispose();
            if (this.jobIndexer != null)
                this.jobInitiatiator.Dispose();
        }
    }
}
