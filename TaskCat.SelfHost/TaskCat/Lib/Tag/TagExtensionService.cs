using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using TaskCat.Common.Db;
using TaskCat.Data.Entity;
using TaskCat.Job;
using TaskCat.Model.Tag;

namespace TaskCat.Lib.Tag
{
    public class TagExtensionService
    {
        private IDbContext dbContext;
        private IObservable<TagActivity> tagActivitySource;

        public TagExtensionService(IDbContext dbContext, IObservable<TagActivity> tagActivitySource)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));
            if (tagActivitySource == null)
                throw new ArgumentNullException(nameof(tagActivitySource));

            this.dbContext = dbContext;
            this.tagActivitySource = tagActivitySource;

            this.tagActivitySource
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Subscribe(OnNext, OnError);
        }

        private void OnNext(TagActivity activity)
        {
            switch(activity.Operation)
            {
                case TagOperation.DELETE:
                    DeleteTagFromJobs(activity.Value);
                    break;
            }
        }

        private void DeleteTagFromJobs(DataTag value)
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
