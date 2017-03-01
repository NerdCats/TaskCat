namespace TaskCat.Account
{
    using Autofac;
    using Autofac.Integration.WebApi;
    using Common.Db;
    using Common.Owin;
    using Common.Settings;
    using Common.Utility.ActionFilter;
    using Core;
    using Data.Entity.Identity;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.DataHandler.Encoder;
    using Microsoft.Owin.Security.Jwt;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Driver;
    using NerdCats.Owin;
    using Owin;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;
    using AppSettings = Its.Configuration.Settings;

    public static class Startup
    {
        public static void ConfigureApp(IAppBuilder app, IContainer container)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            SetupMongoConventions();

            AutofacContainerBuilder builder = new AutofacContainerBuilder();

            app.UseAutofacMiddleware(container);
            app.Use(typeof(PreflightRequestsHandler));
            app.UseForwardHeaders(options: default(ForwardedHeadersOptions));

            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(container);

            var config = new HttpConfiguration();

            ConfigureResourceOAuth(app, container);

            WebApiConfig.Register(config, webApiDependencyResolver);
            config.Filters.Add(new ErrorDocumentFilter());
         
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(config);
            app.UseAutofacWebApi(config);

            EmailTemplatesConfig.Configure();

            // FIXME: Need to move these with other startups
            // This is not ideal
            InitializeRoles(container);

            //FIXME: Can be a small middleware. No? Alright!
            app.Run(context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync(string.Format($"Welcome to TaskCat Account '{version}', proudly baked by NerdCats"));
            });
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

        private static void InitializeRoles(IContainer container)
        {
            var dbContext = container.Resolve<IDbContext>();

            if (dbContext.Roles.Count(Builders<Role>.Filter.Empty) == 0)
            {
                dbContext.Roles.InsertOne(new Role()
                {
                    Name = RoleNames.ROLE_USER
                });

                dbContext.Roles.InsertOne(new Role()
                {
                    Name = RoleNames.ROLE_ENTERPRISE
                });

                dbContext.Roles.InsertOne(new Role()
                {
                    Name = RoleNames.ROLE_ADMINISTRATOR
                });

                dbContext.Roles.InsertOne(new Role()
                {
                    Name = RoleNames.ROLE_ASSET
                });

                dbContext.Roles.InsertOne(new Role()
                {
                    Name = RoleNames.ROLE_BACKOFFICEADMIN
                });
            }
        }
    }
}