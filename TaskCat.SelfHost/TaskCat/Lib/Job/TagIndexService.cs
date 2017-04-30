namespace TaskCat.Lib.Job
{
    using Common.Db;
    using Data.Model.Tag;
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    public class TagIndexService
    {
        private IDbContext dbcontext;
        private IObservable<TagIndexOperation> tagActivitySource;

        public TagIndexService(
            IDbContext dbcontext,
            IObservable<TagIndexOperation> tagActivitySource)
        {
            if (dbcontext == null)
                throw new ArgumentNullException(nameof(dbcontext));
            if (tagActivitySource == null)
                throw new ArgumentNullException(nameof(tagActivitySource));

            this.dbcontext = dbcontext;
            this.tagActivitySource = tagActivitySource;

            this.tagActivitySource
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Subscribe(OnNext, OnError);
        }

        private void OnNext(TagIndexOperation activity)
        {
            throw new NotImplementedException();
        }

        private void OnError(Exception exception)
        {
            Console.WriteLine(exception);
            // TODO: Log the exception here and we might need to redo things here
        }
    }
}
