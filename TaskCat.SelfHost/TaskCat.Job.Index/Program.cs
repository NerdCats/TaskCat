using Dichotomy;
using Dichotomy.Configuration;
using Dichotomy.Helpers;

namespace TaskCat.Job.Index
{
    public class Program
    {
        /// <summary>
        /// This is the entry point of the service host process
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            ServiceManager.Initialize("TaskCatJobIndexService", "TaskCat Job Indexing Service", "This is the job indexing service of TaskCat");

            var config = new ConfigurationOptions("--");
            var runner = new Runner(new TaskCatJobIndexService(), config);
            runner.Run();
        }
    }
}
