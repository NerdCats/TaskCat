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
    using Lib.Email.SendWithUs;
    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            DbContext context = new DbContext();

            builder.Register<DbContext>(c => context).As<IDbContext>().SingleInstance();

            builder.RegisterType<HRIDService>().AsImplementedInterfaces<IHRIDService, ConcreteReflectionActivatorData>();
            builder.RegisterType<PaymentManager>().AsImplementedInterfaces<IPaymentManager, ConcreteReflectionActivatorData>();
            builder.RegisterType<PaymentService>().AsImplementedInterfaces<IPaymentService, ConcreteReflectionActivatorData>();

            builder.RegisterType<SendWithUsMailService>().AsImplementedInterfaces<IMailService, ConcreteReflectionActivatorData>();

            builder.RegisterType<JobStore>().SingleInstance();
            builder.RegisterType<JobManager>().SingleInstance();

            builder.RegisterType<OrderRepository>().SingleInstance();
            builder.RegisterType<OrderRepository>().AsImplementedInterfaces<IOrderRepository, ConcreteReflectionActivatorData>();

            builder.RegisterType<RoleManager>()
                .SingleInstance();

            builder.Register(c=> new AccountStore(context.Users)).As<IUserStore<User>>().SingleInstance();
            builder.RegisterType<AccountManager>().SingleInstance();

            builder.RegisterType<SupportedOrderStore>().SingleInstance();

            builder.Register(c=>new BlobService()).As<IBlobService>().SingleInstance();
            

            builder.RegisterType<AccountRepository>()
                .SingleInstance();

            builder.RegisterType<DefaultAssetProvider>().AsImplementedInterfaces<IAssetProvider, ConcreteReflectionActivatorData>();

            builder.RegisterType<StorageRepository>().AsImplementedInterfaces<IStorageRepository, ConcreteReflectionActivatorData>();

            builder.RegisterType<SimpleAuthorizationServerProvider>()
                .AsImplementedInterfaces<IOAuthAuthorizationServerProvider, ConcreteReflectionActivatorData>().SingleInstance();

            builder.RegisterType<SimpleRefreshTokenProvider>()
                .AsImplementedInterfaces<IAuthenticationTokenProvider, ConcreteReflectionActivatorData>().SingleInstance();


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