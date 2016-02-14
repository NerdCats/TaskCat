namespace TaskCat
{
    using Autofac.Integration.WebApi;
    using Newtonsoft.Json;
    using Swashbuckle.Application;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Web.Http;
    using TaskCat.Lib.Utility.Converter;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, AutofacWebApiDependencyResolver resolver)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.DependencyResolver = resolver;

            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json-patch+json"));
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new OrderModelConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new UserRegistrationModelConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new UserProfileConverter());
            config.Formatters.JsonFormatter.Indent = true;

            
            config.EnableSwagger("docs/{apiVersion}/", c => {

                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory + @"bin\";
                var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);

                c.IncludeXmlComments(commentsFile);

                c.SingleApiVersion("v1", "TaskCat Core Api");
            } ).EnableSwaggerUi();

        }
    }
}
