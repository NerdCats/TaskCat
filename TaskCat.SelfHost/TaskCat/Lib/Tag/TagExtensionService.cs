namespace TaskCat.Lib.Tag
{
    using MongoDB.Driver;
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Common.Db;
    using Data.Entity;
    using Model.Tag;
    using MongoDB.Bson;

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
                    UpdateTagsInJobs(activity.Value, activity.OldValue);
                    break;
                default:
                    throw new NotImplementedException($"{activity.Operation} is not supported for extension works");
            }
        }

        private void UpdateTagsInJobs(DataTag tag, DataTag oldTag)
        {
            var matchFilter = new BsonDocument($"{nameof(Job.Tasks)}", oldTag.Id);
            var updateFilter = new BsonDocument {
                { "$set", new BsonDocument {
                            { $"{nameof(Job.Tasks)}.$", tag.Id }
                        }
                }
            };

            // TODO: need a logger here. Need to log if anything fails like miserably
            var result = this.dbContext.Jobs.UpdateMany(matchFilter, updateFilter);
        }

        private void DeleteTagFromJobs(DataTag tag)
        {
            // TODO: need a logger here. Need to log if anything fails like miserably
            var updateFilter = Builders<Job>.Update.Pull(x => x.Tags, tag.Id);
            var jobFilter = Builders<Job>.Filter.Where(x => x.Tags != null);

            var result = this.dbContext.Jobs.UpdateMany(jobFilter, updateFilter);
        }

        private void OnError(Exception exception)
        {
            Console.WriteLine(exception);
            // TODO: Log the exception here and we might need to redo things here
        }
    }
}
