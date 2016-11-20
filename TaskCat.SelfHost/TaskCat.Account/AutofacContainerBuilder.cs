namespace TaskCat.Account
{
    using Autofac;
    using Autofac.Builder;
    using Autofac.Integration.WebApi;
    using Microsoft.AspNet.Identity;
    using Owin;
    using Core;
    using Common.Db;
    using Common.Email;
    using Common.Storage;
    using Data.Entity.Identity;
    using Common.Email.SMTP;
    using AppSettings = Its.Configuration.Settings;
    using Common.Settings;
    using Lib.Db;
    using Microsoft.ServiceBus;

    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            #region Account

            builder.RegisterType<AccountDbContext>().As<IDbContext>().SingleInstance();
            builder.RegisterType<AccountStore>().As<IUserStore<User>>().SingleInstance();
            builder.RegisterType<AccountManager>().SingleInstance();
            builder.RegisterType<AccountContext>().As<IAccountContext>().SingleInstance();
            builder.RegisterType<RoleManager>().SingleInstance();
            builder.RegisterType<ClientStore>().As<IClientStore>().SingleInstance();
            #endregion

            #region Mail
            var mailService = new SMTPMailService(AppSettings.Get<SMTPMailSettings>(), AppSettings.Get<ProprietorSettings>());
            builder.Register(m => mailService).As<IEmailService>().SingleInstance();
            #endregion

            #region Storage
            builder.Register(c => new BlobService()).As<IBlobService>().SingleInstance();
            builder.RegisterType<StorageRepository>().AsImplementedInterfaces<IStorageRepository, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region ServiceBus
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(AppSettings.Get<ClientSettings>().ServiceBusConnectionString);
            #endregion

            builder.RegisterApiControllers(typeof(Startup).Assembly);
            return builder.Build();
        }
    }
}