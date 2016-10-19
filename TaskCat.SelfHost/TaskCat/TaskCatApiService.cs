namespace TaskCat
{
    using System;
    using Dichotomy;
    using Microsoft.Owin.Hosting;
    using Lib.Constants;
    using Its.Configuration;
    using App.Settings;

    public class TaskCatApiService : IDichotomyService
    {
        private string listeningAddress;
        private IDisposable webApp;

        public TaskCatApiService()
        {
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
            Console.WriteLine("Starting TaskCat Api Service");

            this.webApp = WebApp.Start(listeningAddress, appBuilder => Startup.ConfigureApp(appBuilder));

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Hosted TaskCat on {listeningAddress}");
            Console.ForegroundColor = oldColor;
        }

        public void Stop()
        {
            Console.WriteLine("Stopping TaskCat Api Service");
        }
    }
}
