﻿using System;
using Microsoft.Owin;
using Owin;
using System.Configuration;
using Autofac.Integration.WebApi;
using System.Web.Http;
using Autofac;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Infrastructure;
using MongoDB.Driver;

[assembly: OwinStartup(typeof(TaskCat.App.Startup))]
namespace TaskCat.App
{
    using App_Start;
    using Lib.Db;
    using Lib.Utility;
    using Data.Entity.Identity;
    using Data.Model.Identity;
    using Lib.Utility.ActionFilter;
    using System.Reflection;
    using Microsoft.Owin.Security.Facebook;
    using Lib.Identity;
    using Settings;
    using AppSettings = Its.Configuration.Settings;
    using Lib.Owin;
    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Bson;
    public class Startup
    {
        public void Configuration(IAppBuilder app)
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

#if DEBUG
            AppSettings.Precedence = new[] { "local", "production" };
#else
            AppSettings.Precedence = new[] { "production", "local" };
#endif

            SetupMongoConventions();

            AutofacContainerBuilder builder = new AutofacContainerBuilder();

            var container = builder.BuildContainer(app);
            app.UseAutofacMiddleware(container);
            app.Use(typeof(PreflightRequestsHandler));

            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(container);

            
            var config = new HttpConfiguration();
            

            BsonSerializerConfig.Configure();           

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

        private void SetupMongoConventions()
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

        private void ConfigureOAuth(IAppBuilder app, IContainer container)
        {
            var OAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(2),
                Provider = container.Resolve<IOAuthAuthorizationServerProvider>(),
                RefreshTokenProvider = container.Resolve<IAuthenticationTokenProvider>()
            };


            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            // Generating Token with Providers
            app.UseOAuthAuthorizationServer(OAuthServerOptions);

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

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

        private void InitializeClients(IContainer container)
        {
            var dbContext = container.Resolve<IDbContext>();

            //FIXME: I know, utter stupidity here, need a script to do this       
            if (dbContext.Clients.Count(Builders<Client>.Filter.Empty) == 0)
            {
                // Inserting clients if they are not initialized
                dbContext.Clients.InsertOne(new Client
                {
                    Id = "GoFetchWebApp",
                    Secret = HashMaker.GetHash("GoFetchWebApp@gobd"),
                    Name = "Go Fetch App powered by TaskCat, You are on Web",
                    ApplicationType = ApplicationTypes.JavaScript,
                    Active = false,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "*"
                });

                dbContext.Clients.InsertOne(new Client
                {
                    Id = "GoFetchDevWebApp",
                    Secret = HashMaker.GetHash("GoFetchDevWebApp@gobd"),
                    Name = "Go Fetch App powered by TaskCat, You are on web and in development mode !",
                    ApplicationType = ApplicationTypes.JavaScript,
                    Active = true,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "*"
                });

                dbContext.Clients.InsertOne(new Client
                {
                    Id = "GoFetchDevDroidApp",
                    Secret = HashMaker.GetHash("GoFetchDevDroidApp@gobd"),
                    Name = "Go Fetch App powered by TaskCat, You are one android and in development mode !",
                    ApplicationType = ApplicationTypes.Android,
                    Active = true,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "*"
                });

                dbContext.Clients.InsertOne(new Client
                {
                    Id = "GoFetchDevDroidAssetApp",
                    Secret = HashMaker.GetHash("GoFetchDevDroidAssetApp@gobd"),
                    Name = "Go Fetch App powered by TaskCat, You are one android asset and in development mode !",
                    ApplicationType = ApplicationTypes.Android,
                    Active = true,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "*"
                });

                dbContext.Clients.InsertOne(new Client
                {
                    Id = "ConsoleApp",
                    Secret = HashMaker.GetHash("ConsoleApp@gobd"),
                    Name = "Console Application",
                    ApplicationType = ApplicationTypes.NativeConfidential,
                    Active = true,
                    RefreshTokenLifeTime = 14400,
                    AllowedOrigin = "*"
                });
            }

        }

        private void InitializeRoles(IContainer container)
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

