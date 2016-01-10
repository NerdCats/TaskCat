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
    using Data.Entity.Assets;

    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            switch(ConfigurationManager.AppSettings["ENV"])
            {
                case "mock":
                    builder.RegisterType<FakeJobRepository>().AsImplementedInterfaces<IJobRepository, ConcreteReflectionActivatorData>();
                    builder.RegisterType<FakeNearestRideProvider>().AsImplementedInterfaces<INearestAssetProvider<Ryde>, ConcreteReflectionActivatorData>();
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