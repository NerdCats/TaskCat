namespace TaskCat
{
    using System;
    using Dichotomy;
    using Microsoft.Owin.Hosting;
    using Lib.Constants;
    using Its.Configuration;
    using Common.Settings;
    using Data.Entity;
    using App;
    using Autofac;
    using Lib.Job;
    using Common.Db;
    using System.Reactive.Subjects;
    using System.Diagnostics;
    using Common.Search;
    using Lib.Tag;
    using Model.Tag;

    public class TaskCatApiService : IDichotomyService
    {
        private IContainer container;
        private string listeningAddress;
        private IDisposable webApp;

        private JobActivityService jobActivityService;
        private JobSearchIndexService jobIndexService;
        private TagExtensionService tagActivityService;

        public TaskCatApiService()
        {

#if DEBUG
            Settings.Precedence = new[] { "local", "production" };
#else
            Settings.Precedence = new[] { "production", "local" };
#endif

            this.listeningAddress = string.IsNullOrWhiteSpace(Settings.Get<ClientSettings>().HostingAddress)
                ? AppConstants.DefaultHostingAddress : Settings.Get<ClientSettings>().HostingAddress;
        }

        private void BuildAutofacContainerAndStartActivityService()
        {
            // INFO: Doing the IoC container building here
            AutofacContainerBuilder builder = new AutofacContainerBuilder();
            this.container = builder.BuildContainer();           
        }

        private void InitializeReactiveServices()
        {
            this.jobActivityService = new JobActivityService(container.Resolve<IDbContext>(), container.Resolve<Subject<JobActivity>>());
            this.jobIndexService = new JobSearchIndexService(container.Resolve<ISearchContext>(), container.Resolve<IObservable<Data.Entity.Job>>());
            this.tagActivityService = new TagExtensionService(container.Resolve<IDbContext>(), container.Resolve<IObservable<TagActivity>>());
        }

        public void Dispose()
        {
            if (webApp != null)
            {
                webApp.Dispose();
            }
            if (container != null)
            {
                container.Dispose();
            }
        }

        public void Start()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Console.WriteLine("Starting TaskCat Api Service");

            Console.WriteLine("Building Container...");
            BuildAutofacContainerAndStartActivityService();
            InitializeReactiveServices();          

            this.webApp = WebApp.Start(listeningAddress, appBuilder => Startup.ConfigureApp(appBuilder, container));

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;

            watch.Stop();

            Console.WriteLine($"Hosted TaskCat on {listeningAddress} in {watch.Elapsed.TotalSeconds} seconds");
            Console.ForegroundColor = oldColor;
        }

        public void Stop()
        {
            Console.WriteLine("Stopping TaskCat Api Service");
        }
    }
}
