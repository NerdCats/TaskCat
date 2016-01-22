using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TaskCat.Lib.Utility;

namespace TaskCat
{
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

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new OrderModelConverter());
            config.Formatters.JsonFormatter.Indent = true;
        }
    }
}
