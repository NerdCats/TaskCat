using Microsoft.Azure.ServiceBus;
using System;

namespace TaskCat.BackgroundJobService
{
    public class ServiceBusContext
    {
        private readonly string connectionString;
        private readonly string pushQueueName;
        private readonly string pullQueueName;

        public ServiceBusContext(string connectionString, string pushQueueName, string pullQueueName)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.pushQueueName = pushQueueName ?? throw new ArgumentNullException(nameof(pushQueueName));
            this.pullQueueName = pullQueueName ?? throw new ArgumentNullException(nameof(pullQueueName));
        }

        public QueueClient PushQueueClient => lazyPushQueueClient.Value;

        private Lazy<QueueClient> lazyPushQueueClient =>
            new Lazy<QueueClient>(() => new QueueClient(this.connectionString, this.pushQueueName));

        public QueueClient PullQueueClient => lazyPullQueueClient.Value;

        private Lazy<QueueClient> lazyPullQueueClient =>
            new Lazy<QueueClient>(() => new QueueClient(this.connectionString, this.pullQueueName));
    }
}
