namespace TaskCat
{
    using Dichotomy;
    using Dichotomy.Configuration;
    using Dichotomy.Helpers;
    using NLog;

    public static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
       
        private static void Main(string[] args)
        {
            ServiceManager.Initialize("TaskCatApiService", "TaskCat Api Service", "This is the api service of TaskCat");
            logger.Info("This is just Info to test TaskCat");
            logger.Info("Starting TaskCat Service, this message is from NLog");
            var config = new ConfigurationOptions("--");
            var runner = new Runner(new TaskCatApiService(), config);
            runner.Run();
        }
    }
}
