﻿namespace TaskCat.Auth
{
    using Autofac;
    using Autofac.Builder;
    using Autofac.Integration.WebApi;
    using Common.Db;
    using Common.Email;
    using Common.Storage;
    using Core;
    using Data.Entity.Identity;
    using Lib;
    using Lib.Db;
    using Lib.Email.SMTP;
    using Lib.Provider;
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security.Infrastructure;
    using Microsoft.Owin.Security.OAuth;
    using Owin;

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
            builder.RegisterType<SMTPMailService>().AsImplementedInterfaces<IEmailService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Storage
            builder.Register(c => new BlobService()).As<IBlobService>().SingleInstance();
            builder.RegisterType<StorageRepository>().AsImplementedInterfaces<IStorageRepository, ConcreteReflectionActivatorData>().InstancePerLifetimeScope();
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