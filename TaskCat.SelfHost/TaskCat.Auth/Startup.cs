﻿namespace TaskCat.Auth
{
    using Autofac.Integration.WebApi;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Conventions;
    using Owin;
    using System.Reflection;
    using System.Web.Http;
    using TaskCat.Common.Owin;
    using Autofac;
    using System;
    using Microsoft.Owin.Security.OAuth;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Infrastructure;
    using Its.Configuration;
    using AppSettings = Its.Configuration.Settings;
    using Settings;
    using App.Settings;
    using Microsoft.Owin.Security.Facebook;
    using Lib.JWT;
    using Lib.Provider;
    using Common.Utility.ActionFilter;
    using Lib;
    using Data.Model;
    using Data.Model.Identity;
    using Lib.Db;
    using Data.Entity.Identity;
    using MongoDB.Driver;
    using Common.Db;
    using Core;

    public static class Startup
    {

        public static void ConfigureApp(IAppBuilder app)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            SetupMongoConventions();

            AutofacContainerBuilder builder = new AutofacContainerBuilder();

            app.UseErrorPage();

            var container = builder.BuildContainer(app);
            app.UseAutofacMiddleware(container);
            app.Use(typeof(PreflightRequestsHandler));
            
            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(container);

            var config = new HttpConfiguration();

            ConfigureOAuth(app, container);

            WebApiConfig.Register(config, webApiDependencyResolver);
            config.Filters.Add(new ErrorDocumentFilter());


            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(config);
            app.UseAutofacWebApi(config);

            EmailTemplatesConfig.Configure();

            // FIXME: Need to move these with other startups
            // This is not ideal
            InitializeClients(container);
            InitializeRoles(container);

            //FIXME: Can be a small middleware. No? Alright!
            app.Run(context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync(string.Format($"Welcome to TaskCat '{version}', proudly baked by NerdCats"));
            });
        }

        private static void ConfigureOAuth(IAppBuilder app, IContainer container)
        {
            var OAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("api/auth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(2),
                Provider = container.Resolve<IOAuthAuthorizationServerProvider>(),
                AccessTokenFormat = new TaskCatJWTFormat(AppSettings.Get<ClientSettings>().AuthenticationIssuerName, container.Resolve<IClientStore>()),
                RefreshTokenProvider = container.Resolve<IAuthenticationTokenProvider>()
            };

            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            // Generating Token with Providers
            app.UseOAuthAuthorizationServer(OAuthServerOptions);

            var externalLoginSettings = AppSettings.Get<ExternalLoginSettings>();
            if (externalLoginSettings != null && externalLoginSettings.Facebook != null)
            {
                var facebookAuthOptions = new FacebookAuthenticationOptions()
                {
                    AppId = externalLoginSettings.Facebook.AppId,
                    AppSecret = externalLoginSettings.Facebook.AppSecret,
                    Provider = new FacebookAuthProvider()
                };

                app.UseFacebookAuthentication(facebookAuthOptions);
            }
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

        private static void InitializeClients(IContainer container)
        {
            var clientStore = container.Resolve<IClientStore>();
            var clientsCount = clientStore.GetClientsCount().GetAwaiter().GetResult();

            if (clientsCount == 0)
            {
                clientStore.AddClient(new ClientModel()
                {
                    Id = "GoFetchWebApp",
                    Active = true,
                    AllowedOrigin = "*",
                    ApplicationType = ApplicationTypes.JavaScript,
                    Name = "GoFetchWebApp",
                    RefreshTokenLifeTime = 7200
                }).GetAwaiter().GetResult();

                clientStore.AddClient(new ClientModel()
                {
                    Id = "GoFetchDevWebApp",
                    Active = true,
                    AllowedOrigin = "*",
                    ApplicationType = ApplicationTypes.JavaScript,
                    Name = "GoFetchDevWebApp",
                    RefreshTokenLifeTime = 7200
                }).GetAwaiter().GetResult();

                clientStore.AddClient(new ClientModel()
                {
                    Id = "GoFetchDevDroidApp",
                    Active = true,
                    AllowedOrigin = "*",
                    ApplicationType = ApplicationTypes.Android,
                    Name = "GoFetchDevDroidApp",
                    RefreshTokenLifeTime = 7200
                }).GetAwaiter().GetResult();

                clientStore.AddClient(new ClientModel()
                {
                    Id = "GoFetchDevDroidAssetApp",
                    Active = true,
                    AllowedOrigin = "*",
                    ApplicationType = ApplicationTypes.Android,
                    Name = "GoFetchDevDroidAssetApp",
                    RefreshTokenLifeTime = 7200
                }).GetAwaiter().GetResult();

                clientStore.AddClient(new ClientModel()
                {
                    Id = "ConsoleApp",
                    Active = true,
                    AllowedOrigin = "*",
                    ApplicationType = ApplicationTypes.Android,
                    Name = "ConsoleApp",
                    RefreshTokenLifeTime = 7200
                }).GetAwaiter().GetResult();
            }
        }
    }
}