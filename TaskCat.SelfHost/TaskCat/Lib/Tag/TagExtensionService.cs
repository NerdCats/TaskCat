using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using TaskCat.Data.Entity;
using TaskCat.Job;
using TaskCat.Model.Tag;

namespace TaskCat.Lib.Tag
{
    public class TagExtensionService
    {
        private IJobRepository jobRepository;
        private IObservable<TagActivity> tagActivitySource;

        public TagExtensionService(IJobRepository jobRepository, IObservable<TagActivity> tagActivitySource)
        {
            if (jobRepository == null)
                throw new ArgumentNullException(nameof(jobRepository));
            if (tagActivitySource == null)
                throw new ArgumentNullException(nameof(tagActivitySource));

            this.jobRepository = jobRepository;
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
