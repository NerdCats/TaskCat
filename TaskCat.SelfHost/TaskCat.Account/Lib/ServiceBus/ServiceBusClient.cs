namespace TaskCat.Account.Lib.ServiceBus
{
    using Common.Settings;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using System;

    public class ServiceBusClient : IServiceBusClient
    {
        private NamespaceManager namespaceManager;

        public TopicClient AccountUpdateTopicClient { get; private set; }

        public ServiceBusClient(ClientSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.ServiceBusConnectionString))
                throw new ArgumentException(nameof(settings.ServiceBusConnectionString));

            this.namespaceManager = NamespaceManager.CreateFromConnectionString(settings.ServiceBusConnectionString);

            if (string.IsNullOrWhiteSpace(settings.AccountUpdateTopic))
                throw new ArgumentException(nameof(settings.AccountUpdateTopic));

            if (!namespaceManager.TopicExists(settings.AccountUpdateTopic))
            {
                TopicDescription description = new TopicDescription(settings.AccountUpdateTopic);
                description.DefaultMessageTimeToLive = TimeSpan.FromMinutes(10);
                description.MaxSizeInMegabytes = 1024;
                namespaceManager.CreateTopic(description);
            }

            AccountUpdateTopicClient = TopicClient.CreateFromConnectionString(settings.ServiceBusConnectionString, settings.AccountUpdateTopic);
        }
    }
}
