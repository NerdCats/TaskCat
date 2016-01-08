using Autofac;
using Autofac.Builder;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskCat.Lib.Job;

namespace TaskCat.App
{
    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<JobRepository>().AsImplementedInterfaces<IJobRepository, ConcreteReflectionActivatorData>();
            builder.RegisterApiControllers(typeof(Startup).Assembly);

            return builder.Build();
        }
    }
}