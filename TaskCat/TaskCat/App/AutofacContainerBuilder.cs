namespace TaskCat.App
{
    using Autofac;
    using Autofac.Builder;
    using Autofac.Integration.WebApi;
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
    using Lib.DropPoint;
    using Data.Entity;
    using Lib.Domain;
    using Lib.Catalog;
    using Lib.Vendor;
    using Lib.Comments;

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
            builder.RegisterType<AccountContext>().As<IAccountContext>().InstancePerLifetimeScope();
            builder.RegisterType<RoleManager>().InstancePerLifetimeScope();
            #endregion

            #region Payment
            builder.RegisterType<PaymentManager>().AsImplementedInterfaces<IPaymentManager, ConcreteReflectionActivatorData>().SingleInstance();
            builder.RegisterType<PaymentService>().AsImplementedInterfaces<IPaymentService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Mail
            builder.RegisterType<SMTPMailService>().AsImplementedInterfaces<IEmailService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Job
            builder.RegisterType<JobStore>().InstancePerLifetimeScope();
            builder.RegisterType<JobManager>().AsImplementedInterfaces<IJobManager, ConcreteReflectionActivatorData>().InstancePerLifetimeScope();
            #endregion

            #region Order
            builder.RegisterType<SupportedOrderStore>().InstancePerLifetimeScope();
            builder.RegisterType<OrderRepository>().AsImplementedInterfaces<IOrderRepository, ConcreteReflectionActivatorData>().InstancePerLifetimeScope();
            #endregion

            #region Storage
            builder.Register(c => new BlobService()).As<IBlobService>().SingleInstance();
            builder.RegisterType<StorageRepository>().AsImplementedInterfaces<IStorageRepository, ConcreteReflectionActivatorData>().InstancePerLifetimeScope();
            #endregion

            #region Asset
            builder.RegisterType<DefaultAssetProvider>().AsImplementedInterfaces<IAssetProvider, ConcreteReflectionActivatorData>();
            #endregion

            #region Auth
            builder.RegisterType<TaskCatAuthorizationServerProvider>()
                .AsImplementedInterfaces<IOAuthAuthorizationServerProvider, ConcreteReflectionActivatorData>().SingleInstance();

            builder.RegisterType<TaskCatRefreshTokenProvider>()
                .AsImplementedInterfaces<IAuthenticationTokenProvider, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Hrid
            builder.RegisterType<HRIDService>().AsImplementedInterfaces<IHRIDService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region DropPoint
            builder.RegisterType<DropPointService>().AsImplementedInterfaces<IDropPointService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Vendor
            builder.RegisterType<VendorService>().AsImplementedInterfaces<IVendorService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Catalog
            builder.RegisterType<ProductCategoryService>().AsImplementedInterfaces<IRepository<ProductCategory>, ConcreteReflectionActivatorData>().SingleInstance();
            builder.RegisterType<ProductService>().AsImplementedInterfaces<IRepository<Product>, ConcreteReflectionActivatorData>().SingleInstance();
            builder.RegisterType<StoreService>().AsImplementedInterfaces<IRepository<Store>, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Comment
            builder.RegisterType<CommentService>().AsImplementedInterfaces<ICommentService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            builder.RegisterType<JobRepository>().AsImplementedInterfaces<IJobRepository, ConcreteReflectionActivatorData>();

            builder.RegisterApiControllers(typeof(Startup).Assembly);
            return builder.Build();
        }
    }
}