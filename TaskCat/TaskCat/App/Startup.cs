using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Newtonsoft.Json;
using System.Configuration;
using Autofac.Integration.WebApi;
using System.Web.Http;

[assembly: OwinStartup(typeof(TaskCat.App.Startup))]

namespace TaskCat.App
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Properties["host.AppName"] = ConfigurationManager.AppSettings["AppName"];
            app.Properties["host.AppMode"] = ConfigurationManager.AppSettings["ENV"];

            //FIXME: We need a module to load development/production mode so error pages can be turned on/off
            //Better have a global configuration module like Asp.net 5, that looked awesome!
            switch(app.Properties["host.AppMode"].ToString() )
            {
                case ("development"):
                case ("mock"):
                    app.UseErrorPage();
                    break;
            }

            AutofacContainerBuilder builder = new AutofacContainerBuilder();

            var container = builder.BuildContainer();
            app.UseAutofacMiddleware(container);

            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(container);

            var config = new HttpConfiguration();

            WebApiConfig.Register(config, webApiDependencyResolver);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(config);
            app.UseAutofacWebApi(config);

            //FIXME: Can be a small middleware. No? Alright!
            app.Run(context => {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("Welcome to TaskCat, proudly baked by NerdCats");
            });

        }
    }
}
