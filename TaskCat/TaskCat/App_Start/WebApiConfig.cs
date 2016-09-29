﻿namespace TaskCat
{
    using Autofac.Integration.WebApi;
    using Newtonsoft.Json;
    using Swashbuckle.Application;
    using System;
    using System.IO;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.OData.Extensions;
    using TaskCat.Lib.Utility.Converter;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, AutofacWebApiDependencyResolver resolver)
        {
            config.EnableEnumPrefixFree(enumPrefixFree: true);
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.DependencyResolver = resolver;

            ConfigureFormatters(config);

            config.EnableSwagger("docs/{apiVersion}/", c =>
            {

                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory + @"bin\";
                var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);

                c.IncludeXmlComments(commentsFile);
                c.DescribeAllEnumsAsStrings();
                c.SingleApiVersion("v1", "TaskCat Core Api");
            }).EnableSwaggerUi();

        }

        private static void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            config.Formatters.JsonFormatter.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json-patch+json"));
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new OrderModelConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new RegistrationModelConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new UserProfileConverter());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new VendorConverter());
            config.Formatters.JsonFormatter.Indent = true;
        }
    }
}
