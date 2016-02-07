namespace TaskCat.App
{
    using Autofac;
    using Autofac.Builder;
    using Autofac.Integration.WebApi;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using Lib.Asset;
    using TaskCat.Lib.Job;
    using Lib.Db;
    using Lib.Order;
    using Data.Entity.Identity;
    using AspNet.Identity.MongoDB;
    using Microsoft.AspNet.Identity;
    using Lib.Identity;
    using Microsoft.Owin.Security.OAuth;
    using Microsoft.Owin.Security.Infrastructure;
    using Lib.Auth;
    using Data.Entity;
    using MongoDB.Driver;

    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            DbContext context = new DbContext();

            builder.Register<DbContext>(c => context).As<IDbContext>().SingleInstance();

            builder.RegisterType<JobStore>().SingleInstance();
            builder.RegisterType<JobManager>().SingleInstance();

            builder.RegisterType<OrderRepository>().SingleInstance();
            builder.RegisterType<OrderRepository>().AsImplementedInterfaces<IOrderRepository, ConcreteReflectionActivatorData>();

            builder.RegisterType<RoleManager>()
                .SingleInstance();

            builder.Register(c => new UserManager<User>(new UserStore<User>(context.Users)));
            builder.Register(c => new UserManager<Asset>(new UserStore<Asset>(context.Assets)));

            builder.RegisterType<AuthRepository>()
                .SingleInstance();

            builder.RegisterType<SimpleAuthorizationServerProvider>()
                .AsImplementedInterfaces<IOAuthAuthorizationServerProvider, ConcreteReflectionActivatorData>().SingleInstance();

            builder.RegisterType<SimpleRefreshTokenProvider>()
                .AsImplementedInterfaces<IAuthenticationTokenProvider, ConcreteReflectionActivatorData>().SingleInstance();

            switch (ConfigurationManager.AppSettings["ENV"])
            {
                case "mock":
                    builder.RegisterType<FakeJobRepository>().AsImplementedInterfaces<IJobRepository, ConcreteReflectionActivatorData>();
                    builder.RegisterType<FakeNearestRideProvider>().AsImplementedInterfaces<INearestAssetProvider<Asset>, ConcreteReflectionActivatorData>();
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