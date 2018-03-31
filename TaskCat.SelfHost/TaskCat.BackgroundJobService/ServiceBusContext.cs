using Microsoft.Azure.ServiceBus;
using System;

namespace TaskCat.BackgroundJobService
{
    public class ServiceBusContext
    {
        private readonly string connectionString;
        private readonly string queueName;

        public ServiceBusContext(string connectionString, string queueName)
        {
            this.connectionString = connectionString;
            this.queueName = queueName;
        }

        public QueueClient QueueClient => lazyQueueClient.Value;

        private Lazy<QueueClient> lazyQueueClient => 
            new Lazy<QueueClient>(new QueueClient(this.connectionString, this.queueName));
    }
}
