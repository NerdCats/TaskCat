using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TaskCat.BackgroundJobService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHostedService, TimedJobPollingService>();        
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {         
        }
    }
}
