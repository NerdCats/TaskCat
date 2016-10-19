using Dichotomy;
using Dichotomy.Configuration;
using Dichotomy.Helpers;

namespace TaskCat
{
    public static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            ServiceManager.Initialize("TaskCatApiService", "TaskCat Api Service", "This is the api service of TaskCat");

            var config = new ConfigurationOptions("--");
            var runner = new Runner(new TaskCatApiService(), config);
            runner.Run();
        }
    }
}
