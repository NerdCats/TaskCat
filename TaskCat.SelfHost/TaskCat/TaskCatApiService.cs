namespace TaskCat
{
    using System;
    using Dichotomy;
    using Microsoft.Owin.Hosting;
    using Lib.Constants;
    using Its.Configuration;
    using Common.Settings;
    using Data.Entity;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class TaskCatApiService : IDichotomyService
    {
        private Subject<JobActivity> jobActivitySubject;
        private string listeningAddress;
        private IDisposable webApp;

        public TaskCatApiService()
        {

#if DEBUG
            Settings.Precedence = new[] { "local", "production" };
#else
            Settings.Precedence = new[] { "production", "local" };
#endif

            this.listeningAddress = string.IsNullOrWhiteSpace(Settings.Get<ClientSettings>().HostingAddress)
                ? AppConstants.DefaultHostingAddress : Settings.Get<ClientSettings>().HostingAddress;

            // INFO: Damn!
            this.jobActivitySubject = new Subject<JobActivity>();
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

            this.webApp = WebApp.Start(listeningAddress, appBuilder => Startup.ConfigureApp(appBuilder, jobActivitySubject));

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
