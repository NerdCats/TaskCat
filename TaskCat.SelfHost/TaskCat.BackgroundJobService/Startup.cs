using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            var pushQueueName = configuration["pushqueue"];
            var pullQueueName = configuration["pullqueue"];

            var serviceBusContext = new ServiceBusContext(
                connectionString: serviceBusConnectionString, 
                pushQueueName: pushQueueName,
                pullQueueName: pullQueueName);

            services.AddSingleton(serviceBusContext);

            services.AddSingleton<HttpClient>();
            services.AddSingleton<IHostedService, InfiniPollingService>();
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile($"Logs/{nameof(InfiniPollingService)}-{{Date}}.txt");
        }
    }
}
