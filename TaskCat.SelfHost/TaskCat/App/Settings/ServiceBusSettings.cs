namespace TaskCat.App.Settings
{
    public class ServiceBusSettings
    {
        public QueueConfig JobPullConfig { get; set; }
        public QueueConfig JobPushConfig { get; set; }
    }
}
