namespace TaskCat.Warehouse
{
    using Account.Core;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Common.Db;
    using Lib;
    using Owin;

    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WarehouseDbContext>().As<IDbContext>().SingleInstance();
            builder.RegisterType<ClientStore>().As<IClientStore>().SingleInstance();

            builder.RegisterApiControllers(typeof(Startup).Assembly);
            return builder.Build();
        }
    }
}