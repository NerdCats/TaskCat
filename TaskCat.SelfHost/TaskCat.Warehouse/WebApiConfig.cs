namespace TaskCat.Warehouse
{
    using System;
    using System.Web.Http;
    using Autofac.Integration.WebApi;
    using Swashbuckle.Application;
    using System.Reflection;
    using System.IO;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Net.Http.Headers;
    using Common.WebApi;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, AutofacWebApiDependencyResolver webApiDependencyResolver)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/warehouse/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.DependencyResolver = webApiDependencyResolver;

            ConfigureFormatters(config);

            config.EnableSwagger("docs/warehouse/api/{apiVersion}/", c =>
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);

                c.IncludeXmlComments(commentsFile);
                c.DescribeAllEnumsAsStrings();
                c.SingleApiVersion("v1", "TaskCat Account Api");
            }).EnableSwaggerUi("docs/account/{*assetPath}");
        }

        private static void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            config.Formatters.JsonFormatter.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            if (appXmlType != null)
                config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json-patch+json"));

            config.MessageHandlers.Insert(0, new CompressionHandler());

            config.Formatters.JsonFormatter.Indent = true;
        }
    }
}