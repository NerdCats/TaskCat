using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using AppSettings = Its.Configuration.Settings;
using Owin;
using TaskCat.App_Start;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using TaskCat.Account.Core;
using TaskCat.Common.Owin;
using TaskCat.Common.Utility.ActionFilter;
using TaskCat.Common.Settings;
using System.Reactive.Linq;
using TaskCat.Job.Extensions;
using NerdCats.Owin;

namespace TaskCat
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder app, IContainer container)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            app.Properties["host.AppName"] = ConfigurationManager.AppSettings["AppName"];
            app.Properties["host.AppMode"] = ConfigurationManager.AppSettings["ENV"];

            //FIXME: We need a module to load development/production mode so error pages can be turned on/off
            //Better have a global configuration module like Asp.net 5, that looked awesome!
            switch (app.Properties["host.AppMode"].ToString())
            {
                case ("development"):
                case ("mock"):
                    app.UseErrorPage();
                    break;
            }

            SetupMongoConventions();
            SetupJobTaskExtensions();

            app.UseAutofacMiddleware(container);
            app.Use(typeof(PreflightRequestsHandler));
            app.UseForwardHeaders(options: default(ForwardedHeadersOptions));

            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(container);


            var config = new HttpConfiguration();


            BsonSerializerConfig.Configure();

            // INFO: This is not done either
            ConfigureResourceOAuth(app, container);

            WebApiConfig.Register(config, webApiDependencyResolver);
            config.Filters.Add(new ErrorDocumentFilter());


            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(config);
            app.UseAutofacWebApi(config);

            EmailTemplatesConfig.Configure();

            //FIXME: Can be a small middleware. No? Alright!
            app.Run(context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync(string.Format($"Welcome to TaskCat '{version}', proudly baked by NerdCats"));
            });
        }

        private static void SetupJobTaskExtensions()
        {
            DeliveryJobExtensions.RegisterExtensions();
        }

        private static void SetupMongoConventions()
        {
            var pack = new ConventionPack()
            {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("EnumConvensions", pack, t => true);

            ConventionPack nullPack = new ConventionPack();
            nullPack.Add(new IgnoreIfNullConvention(true));

            ConventionRegistry.Register("IgnoreNull", nullPack, type => true);
        }


        private static void ConfigureResourceOAuth(IAppBuilder app, IContainer container)
        {
            // INFO: As this is a auth-server and api-server together, Im adding back all possible added clients in the system and
            // allowing all of them to be able to access this api anyway

            var issuer = AppSettings.Get<ClientSettings>().AuthenticationIssuerName;
            var clientStore = container.Resolve<IClientStore>();

            var allClients = clientStore.GetAllClients().GetAwaiter().GetResult();
            var allowedAudiences = allClients.Select(x => x.Id);
            var issuerSecurityTokenProviders =
                allClients.Select(
                    x => new SymmetricKeyIssuerSecurityTokenProvider(issuer, TextEncodings.Base64Url.Decode(x.Secret)));

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = allowedAudiences,
                    IssuerSecurityTokenProviders = issuerSecurityTokenProviders
                });
        }
    }
}
