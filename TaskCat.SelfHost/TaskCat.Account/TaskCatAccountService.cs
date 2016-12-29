namespace TaskCat.Account
{
    using System;
    using Dichotomy;
    using AppSettings = Its.Configuration.Settings;
    using Common.Settings;
    using Lib.Constants;
    using Microsoft.Owin.Hosting;
    using Propagation;
    using Autofac;
    using Common.Db;
    using Data.Entity.Identity;
    using Data.Model.Identity.Response;

    public class TaskCatAccountService : IDichotomyService
    {
        private IContainer container;
        private string listeningAddress;
        private IDisposable webApp;

        private AccountUpdatePropagationService accountUpdatePropagationService;

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
            if (container != null)
            {
                container.Dispose();
            }
        }

        public void Start()
        {
            Console.WriteLine("Starting TaskCat Account Service");

            Console.WriteLine("Building Container...");
            BuildAutofacContainerAndStartActivityService();
            InitializeReactiveServices();

            this.webApp = WebApp.Start(listeningAddress, appBuilder => Startup.ConfigureApp(appBuilder, container));

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Hosted TaskCat Account Service on {listeningAddress}");
            Console.ForegroundColor = oldColor;
        }

        private void InitializeReactiveServices()
        {
            this.accountUpdatePropagationService = new AccountUpdatePropagationService(
                container.Resolve<IDbContext>(),
                container.Resolve<IObservable<User>>(),
                container.Resolve<IObservable<UserModel>>());
        }

        private void BuildAutofacContainerAndStartActivityService()
        {
            AutofacContainerBuilder builder = new AutofacContainerBuilder();
            this.container = builder.BuildContainer();
        }

        public void Stop()
        {
            Console.WriteLine("Stopping TaskCat Account Service");
        }
    }
}