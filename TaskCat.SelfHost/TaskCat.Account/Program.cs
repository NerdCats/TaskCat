using Dichotomy;
using Dichotomy.Configuration;
using Dichotomy.Helpers;

namespace TaskCat.Account
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServiceManager.Initialize("TaskCatAccountService", "TaskCat Account Service", "This is the account service of TaskCat");

            var config = new ConfigurationOptions("--");
            var runner = new Runner(new TaskCatAccountService(), config);
            runner.Run();
        }
    }
}
