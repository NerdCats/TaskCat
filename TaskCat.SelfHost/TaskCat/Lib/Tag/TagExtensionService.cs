namespace TaskCat.Lib.Tag
{
    using MongoDB.Driver;
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Common.Db;
    using Data.Entity;
    using Model.Tag;

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
            switch (activity.Operation)
            {
                case TagOperation.DELETE:
                    DeleteTagFromJobs(activity.Value);
                    break;
                case TagOperation.UPDATE:
                    UpdateTagsInJobs(activity.Value);
                    break;
                default:
                    throw new NotImplementedException($"{activity.Operation} is not supported for extension works");
            }
        }

        private void UpdateTagsInJobs(DataTag tag)
        {
            throw new NotImplementedException();
        }

        private void DeleteTagFromJobs(DataTag tag)
        {
            // TODO: need a logger here. Need to log if anything fails like miserably
            var updateFilter = Builders<Job>.Update.PullFilter(x => x.Tags, y => y == tag.Id);
            var result = this.dbContext.Jobs.UpdateMany(Builders<Job>.Filter.Empty, updateFilter);
        }

        private void OnError(Exception exception)
        {
            Console.WriteLine(exception);
            // TODO: Log the exception here and we might need to redo things here
        }
    }
}
