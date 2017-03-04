namespace TaskCat.Warehouse
{
    using Account.Core;
    using Autofac;
    using Autofac.Builder;
    using Autofac.Integration.WebApi;
    using Common.Db;
    using Job;
    using Lib;
    using Owin;

    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WarehouseDbContext>().As<IDbContext>().SingleInstance();
            builder.RegisterType<ClientStore>().As<IClientStore>().SingleInstance();

            builder.RegisterType<JobStore>().SingleInstance();
            builder.RegisterType<JobManager>().AsImplementedInterfaces<IJobManager, ConcreteReflectionActivatorData>().SingleInstance();
            builder.RegisterType<JobRepository>().AsImplementedInterfaces<IJobRepository, ConcreteReflectionActivatorData>().SingleInstance();

            builder.RegisterApiControllers(typeof(Startup).Assembly);
            return builder.Build();
        }
    }
}