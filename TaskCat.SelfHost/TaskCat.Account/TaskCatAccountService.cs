namespace TaskCat.Account
{
    using System;
    using Dichotomy;
    using AppSettings = Its.Configuration.Settings;
    using Common.Settings;
    using Lib.Constants;
    using Microsoft.Owin.Hosting;

    public class TaskCatAccountService : IDichotomyService
    {
        private string listeningAddress;
        private IDisposable webApp;

        public TaskCatAccountService()
        {
#if DEBUG
            AppSettings.Precedence = new[] { "local", "production" };
#else
            AppSettings.Precedence = new[] { "production", "local" };
#endif

            this.listeningAddress = string.IsNullOrWhiteSpace(AppSettings.Get<ClientSettings>().HostingAddress)
                ? AppConstants.DefaultHostingAddress : AppSettings.Get<ClientSettings>().HostingAddress;
        }

        public void Dispose()
        {
            if (webApp != null)
            {
                webApp.Dispose();
            }
        }

        public void Start()
        {
            Console.WriteLine("Starting TaskCat Account Service");

            this.webApp = WebApp.Start(listeningAddress, appBuilder => Startup.ConfigureApp(appBuilder));

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Hosted TaskCat Account Service on {listeningAddress}");
            Console.ForegroundColor = oldColor;
        }

        public void Stop()
        {
            Console.WriteLine("Stopping TaskCat Account Service");
        }
    }
}