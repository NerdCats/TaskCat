namespace TaskCat.App
{
    using Autofac;
    using Autofac.Builder;
    using Autofac.Integration.WebApi;
    using Lib.Asset;
    using Lib.Job;
    using Lib.Order;
    using Data.Entity.Identity;
    using Microsoft.AspNet.Identity;
    using Lib.AssetProvider;
    using Lib.HRID;
    using Lib.Payment;
    using Data.Lib.Payment;
    using Lib.Email.SMTP;
    using Owin;
    using Lib.DropPoint;
    using Data.Entity;
    using Lib.Domain;
    using Lib.Catalog;
    using Lib.Vendor;
    using Lib.Comments;
    using Common.Email;
    using Common.Storage;
    using Common.Db;
    using Auth.Core;

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

            #region Payment
            builder.RegisterType<PaymentManager>().AsImplementedInterfaces<IPaymentManager, ConcreteReflectionActivatorData>().SingleInstance();
            builder.RegisterType<PaymentService>().AsImplementedInterfaces<IPaymentService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Mail
            builder.RegisterType<SMTPMailService>().AsImplementedInterfaces<IEmailService, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Job
            builder.RegisterType<JobStore>().InstancePerLifetimeScope();
            builder.RegisterType<JobManager>().AsImplementedInterfaces<IJobManager, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Order
            builder.RegisterType<SupportedOrderStore>().InstancePerLifetimeScope();
            builder.RegisterType<OrderRepository>().AsImplementedInterfaces<IOrderRepository, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Storage
            builder.Register(c => new BlobService()).As<IBlobService>().SingleInstance();
            builder.RegisterType<StorageRepository>().AsImplementedInterfaces<IStorageRepository, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Asset
            builder.RegisterType<DefaultAssetProvider>().AsImplementedInterfaces<IAssetProvider, ConcreteReflectionActivatorData>().SingleInstance();
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

            builder.RegisterType<JobRepository>().AsImplementedInterfaces<IJobRepository, ConcreteReflectionActivatorData>().SingleInstance();

            builder.RegisterApiControllers(typeof(Startup).Assembly);
            return builder.Build();
        }
    }
}