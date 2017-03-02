namespace TaskCat.Warehouse
{
    using Dichotomy.Configuration;
    using Dichotomy.Helpers;
    using Dichotomy;

    public static class Program
    {
        private static void Main(string[] args)
        {
            ServiceManager.Initialize("TaskCatWarehouseService", "TaskCat Warehouse Service", "This is the api service of TaskCat warehouse microservice");
            var config = new ConfigurationOptions("--");
            var runner = new Runner(new TaskCatWarehouseService(), config);
            runner.Run();
        }
    }
}
