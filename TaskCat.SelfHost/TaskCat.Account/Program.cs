using Dichotomy;
using Dichotomy.Configuration;
using Dichotomy.Helpers;

namespace TaskCat.Account
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServiceManager.Initialize("TaskCatAccountService", "TaskCat Authentication Service", "This is the authentication service of TaskCat");

            var config = new ConfigurationOptions("--");
            var runner = new Runner(new TaskCatAccountService(), config);
            runner.Run();
        }
    }
}
