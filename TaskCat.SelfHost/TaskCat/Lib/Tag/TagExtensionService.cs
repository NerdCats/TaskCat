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
    using NLog;

    public class TagExtensionService
    {
        private IDbContext dbContext;
        private IObservable<TagActivity> tagActivitySource;
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            // INFO: If this magic looks confusing, please visit https://docs.mongodb.com/manual/reference/operator/update/set/

            var matchFilter = new BsonDocument($"{nameof(Job.Tags)}", oldTag.Value);
            var updateFilter = new BsonDocument {
                { "$set", new BsonDocument {
                            { $"{nameof(Job.Tags)}.$", tag.Value }
                        }
                }
            };

            var result = this.dbContext.Jobs.UpdateMany(matchFilter, updateFilter);
        }

        private void DeleteTagFromJobs(DataTag tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            var updateFilter = Builders<Job>.Update.Pull(x => x.Tags, tag.Value);
            var jobFilter = Builders<Job>.Filter.Where(x => x.Tags != null);
            var result = this.dbContext.Jobs.UpdateMany(jobFilter, updateFilter);
        }

        private void OnError(Exception exception)
        {
            Console.WriteLine(exception);
            logger.Error(exception);
        }
    }
}
