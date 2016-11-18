namespace TaskCat.Auth
{
    using Autofac;
    using Autofac.Builder;
    using Autofac.Integration.WebApi;
    using Common.Db;
    using Account.Core;
    using Data.Entity.Identity;
    using Lib.Provider;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security.Infrastructure;
    using Microsoft.Owin.Security.OAuth;
    using Owin;
    using Common.Email.SMTP;
    using Common.Storage;
    using Common.Email;
    using AppSettings = Its.Configuration.Settings;
    using Common.Settings;

    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            #region Account

            builder.RegisterType<DbContext>().As<IDbContext>().SingleInstance();
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

            #region Auth
            builder.RegisterType<TaskCatOAuthProvider>()
                .AsImplementedInterfaces<IOAuthAuthorizationServerProvider, ConcreteReflectionActivatorData>().SingleInstance();

            builder.RegisterType<TaskCatRefreshTokenProvider>()
                .AsImplementedInterfaces<IAuthenticationTokenProvider, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            builder.RegisterApiControllers(typeof(Startup).Assembly);
            return builder.Build();
        }
    }
}