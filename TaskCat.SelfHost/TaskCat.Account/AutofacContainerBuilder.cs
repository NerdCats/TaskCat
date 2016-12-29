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
    using Lib.ServiceBus;
    using Core.Lib.ServiceBus;
    using System.Reactive.Subjects;
    using Data.Model.Identity.Response;
    using System;

    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            var userSubject = new Subject<User>();
            builder.Register(x => userSubject).As<IObserver<User>>().SingleInstance();
            builder.Register(x => userSubject).As<IObservable<User>>().SingleInstance();

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
            ServiceBusClient serviceBusClient = new ServiceBusClient(AppSettings.Get<ClientSettings>());
            builder.Register(s => serviceBusClient).As<IServiceBusClient>().SingleInstance();
            #endregion

            builder.RegisterApiControllers(typeof(Startup).Assembly);
            return builder.Build();
        }
    }
}