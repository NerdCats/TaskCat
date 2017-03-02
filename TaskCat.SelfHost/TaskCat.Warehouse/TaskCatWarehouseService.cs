namespace TaskCat.Warehouse
{
    using Dichotomy;
    using Its.Configuration;
    using System;
    using System.ComponentModel;
    using Common.Settings;
    using Lib;
    using Microsoft.Owin.Hosting;

    public class TaskCatWarehouseService : IDichotomyService
    {
        private IContainer container;
        private string listeningAddress;
        private IDisposable webApp;

        public TaskCatWarehouseService()
        {
            // TODO: We might need to use a logger here
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler((s, e) => { });

#if DEBUG
            Settings.Precedence = new[] { "local", "production" };
#else
            Settings.Precedence = new[] { "production", "local" };
#endif
            this.listeningAddress = string.IsNullOrWhiteSpace(Settings.Get<ClientSettings>().HostingAddress)
                ? AppConstants.DefaultHostingAddress : Settings.Get<ClientSettings>().HostingAddress;
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
            Console.WriteLine("Starting TaskCat Warehouse Service");

            this.webApp = WebApp.Start(listeningAddress, appBuilder => Startup.ConfigureApp(appBuilder));

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Hosted TaskCat Warehouse Service on {listeningAddress}");
            Console.ForegroundColor = oldColor;
        }


        public void Stop()
        {
            Console.WriteLine("Stopping TaskCat Warehouse Service");
        }
    }
}