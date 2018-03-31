using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace TaskCat.BackgroundJobService
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var redisConnectionString = configuration["kvstore"];
            var redisContext = new RedisContext(redisConnectionString);

            services.AddSingleton<RedisContext>(redisContext);

            var serviceBusConnectionString = configuration["servicebus"];
            var serviceBusQueueName = configuration["queue"];

            var serviceBusContext = new ServiceBusContext(serviceBusConnectionString, serviceBusQueueName);
            services.AddSingleton(serviceBusContext);

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHostedService, InfiniPollingService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }
}
