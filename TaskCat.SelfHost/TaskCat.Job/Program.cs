using Dichotomy;
using Dichotomy.Configuration;
using Dichotomy.Helpers;

namespace TaskCat.Job
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServiceManager.Initialize("TaskCatAuthService", "TaskCat Authentication Service", "This is the authentication service of TaskCat");

            var config = new ConfigurationOptions("--");
            var runner = new Runner(new TaskCatAuthService(), config);
            runner.Run();
        }
    }
}
