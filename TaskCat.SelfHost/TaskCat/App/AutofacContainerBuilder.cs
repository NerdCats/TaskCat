namespace TaskCat.App
{
    using Autofac;
    using Autofac.Builder;
    using Autofac.Integration.WebApi;
    using Lib.Asset;
    using Data.Entity.Identity;
    using Microsoft.AspNet.Identity;
    using Lib.AssetProvider;
    using Data.Lib.Payment;
    using Lib.DropPoint;
    using Data.Entity;
    using Lib.Catalog;
    using Lib.Comments;
    using Common.Email;
    using Common.Storage;
    using Common.Db;
    using Account.Core;
    using Common.Domain;
    using Common.Email.SMTP;
    using AppSettings = Its.Configuration.Settings;
    using Common.Settings;
    using Lib.Db;
    using System.Reactive.Subjects;
    using Common.Search;
    using System;
    using Common.HRID;
    using Payment.Core;
    using Job;
    using Job.Order;
    using Job.Vendor;
    using Lib.Tag;
    using Model.Tag;

    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            var jobActivitySubject = new Subject<JobActivity>();
            builder.Register(x => jobActivitySubject).As<Subject<JobActivity>>().SingleInstance();

            var jobSearchSubject = new Subject<Job>();
            builder.Register(x => jobSearchSubject).As<IObserver<Job>>().SingleInstance();
            builder.Register(x => jobSearchSubject).As<IObservable<Job>>().SingleInstance();

            builder.RegisterType<ApiDbContext>().As<IDbContext>().SingleInstance();
            builder.RegisterType<SearchContext>().As<ISearchContext>().SingleInstance();

            #region Account
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
            var mailService = new SMTPMailService(AppSettings.Get<SMTPMailSettings>(), AppSettings.Get<ProprietorSettings>());
            builder.Register(m => mailService).As<IEmailService>().SingleInstance();
            #endregion

            #region Job

            builder.RegisterType<LocalityService>()
                .AsImplementedInterfaces<ILocalityService, ConcreteReflectionActivatorData>()
                .SingleInstance();
            builder.RegisterType<JobStore>().SingleInstance();
            builder.RegisterType<JobManager>().AsImplementedInterfaces<IJobManager, ConcreteReflectionActivatorData>().SingleInstance();
            builder.RegisterType<JobRepository>().AsImplementedInterfaces<IJobRepository, ConcreteReflectionActivatorData>().SingleInstance();
            #endregion

            #region Order
            builder.RegisterType<SupportedOrderStore>().SingleInstance();
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

            #region Tags                       
            builder.RegisterType<DataTagService>().As<IDataTagService>().SingleInstance();

            var tagsIndexSubject = new Subject<TagActivity>();
            builder.Register(x => tagsIndexSubject).As<IObserver<TagActivity>>().SingleInstance();
            builder.Register(x => tagsIndexSubject).As<IObservable<TagActivity>>().SingleInstance();

            #endregion

            builder.RegisterApiControllers(typeof(Startup).Assembly);
            return builder.Build();
        }
    }
}