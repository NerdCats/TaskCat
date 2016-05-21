namespace TaskCat.App
{
    using Autofac;
    using Autofac.Builder;
    using Autofac.Integration.WebApi;
    using System.Configuration;
    using Lib.Asset;
    using Lib.Job;
    using Lib.Db;
    using Lib.Order;
    using Data.Entity.Identity;
    using Microsoft.AspNet.Identity;
    using Lib.Identity;
    using Microsoft.Owin.Security.OAuth;
    using Microsoft.Owin.Security.Infrastructure;
    using Lib.Auth;
    using Lib.Storage;
    using Lib.AssetProvider;
    using Lib.HRID;
    using Lib.Payment;
    using Data.Lib.Payment;
    using Lib.Email;
    using Lib.Email.SMTP;
    using Owin;
    using Microsoft.Owin.Security.DataProtection;

    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            #region Account
            builder.Register(c => app.GetDataProtectionProvider()).As<IDataProtectionProvider>().SingleInstance();
            builder.RegisterType<DbContext>().As<IDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<AccountStore>().As<IUserStore<User>>().InstancePerLifetimeScope();
            builder.RegisterType<AccountManager>().InstancePerLifetimeScope();
            builder.RegisterType<AccountContext>().InstancePerLifetimeScope();
            builder.RegisterType<RoleManager>().InstancePerLifetimeScope();
            #endregion

            #region Payment
            builder.RegisterType<HRIDService>().AsImplementedInterfaces<IHRIDService, ConcreteReflectionActivatorData>().SingleInstance();
            builder.RegisterType<PaymentManager>().AsImplementedInterfaces<IPaymentManager, ConcreteReflectionActivatorData>().SingleInstance();
            builder.RegisterType<PaymentService>().AsImplementedInterfaces<IPaymentService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Mail
            builder.RegisterType<SMTPMailService>().AsImplementedInterfaces<IEmailService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Job
            builder.RegisterType<JobStore>().InstancePerLifetimeScope();
            builder.RegisterType<JobManager>().InstancePerLifetimeScope();
            #endregion

            #region Order
            builder.RegisterType<SupportedOrderStore>().InstancePerLifetimeScope();
            builder.RegisterType<OrderRepository>().AsImplementedInterfaces<IOrderRepository, ConcreteReflectionActivatorData>().InstancePerLifetimeScope();
            #endregion

            #region Storage
            builder.Register(c=>new BlobService()).As<IBlobService>().SingleInstance();
            builder.RegisterType<StorageRepository>().AsImplementedInterfaces<IStorageRepository, ConcreteReflectionActivatorData>().InstancePerLifetimeScope();
            #endregion

            #region Asset
            builder.RegisterType<DefaultAssetProvider>().AsImplementedInterfaces<IAssetProvider, ConcreteReflectionActivatorData>();
            #endregion

            #region Auth
            builder.RegisterType<SimpleAuthorizationServerProvider>()
                .AsImplementedInterfaces<IOAuthAuthorizationServerProvider, ConcreteReflectionActivatorData>().SingleInstance();

            builder.RegisterType<SimpleRefreshTokenProvider>()
                .AsImplementedInterfaces<IAuthenticationTokenProvider, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            switch (ConfigurationManager.AppSettings["ENV"])
            {
                case "mock":
                    builder.RegisterType<FakeJobRepository>().AsImplementedInterfaces<IJobRepository, ConcreteReflectionActivatorData>();
                    break;
                default:
                    builder.RegisterType<JobRepository>().AsImplementedInterfaces<IJobRepository, ConcreteReflectionActivatorData>();
                    break;
            }

            builder.RegisterApiControllers(typeof(Startup).Assembly);
            return builder.Build();
        }
    }    
}