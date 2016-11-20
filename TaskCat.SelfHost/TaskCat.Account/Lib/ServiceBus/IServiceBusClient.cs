namespace TaskCat.Account.Lib.ServiceBus
{
    using Microsoft.ServiceBus.Messaging;

    public interface IServiceBusClient
    {
        TopicClient AccountUpdateTopicClient { get; }
    }
}