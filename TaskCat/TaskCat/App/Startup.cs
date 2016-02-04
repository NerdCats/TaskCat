﻿using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Newtonsoft.Json;
using System.Configuration;
using Autofac.Integration.WebApi;
using System.Web.Http;
using TaskCat.App_Start;
using Autofac;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Infrastructure;
using TaskCat.Lib.Db;
using TaskCat.Lib.Utility;
using TaskCat.Data.Entity.Identity;
using TaskCat.Data.Model.Identity;
using MongoDB.Driver;

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
            switch (app.Properties["host.AppMode"].ToString())
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

            BsonSerializerConfig.Configure();

            ConfigureOAuth(app, container);

            WebApiConfig.Register(config, webApiDependencyResolver);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(config);
            app.UseAutofacWebApi(config);

            // FIXME: Need to move these with other startups
            // This is not ideal
            InitializeClients(container);
            InitializeRoles(container);

            //FIXME: Can be a small middleware. No? Alright!
            app.Run(context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("Welcome to TaskCat, proudly baked by NerdCats");
            });

        }

        private void ConfigureOAuth(IAppBuilder app, IContainer container)
        {
            var OAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = container.Resolve<IOAuthAuthorizationServerProvider>(),
                RefreshTokenProvider = container.Resolve<IAuthenticationTokenProvider>()
            };

            // Generating Token with Providers
            app.UseOAuthAuthorizationServer(OAuthServerOptions);

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
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
                    AllowedOrigin = "http://gofetch.cloudapp.net"
                });

                dbContext.Clients.InsertOne(new Client
                {
                    Id = "GoFetchDevWebApp",
                    Secret = HashMaker.GetHash("GoFetchDevWebApp@gobd"),
                    Name = "Go Fetch App powered by TaskCat, You are one web and in development mode !",
                    ApplicationType = ApplicationTypes.JavaScript,
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
                    Name = "User"
                });

                dbContext.Roles.InsertOne(new Role()
                {
                    Name = "Administrator"
                });

                dbContext.Roles.InsertOne(new Role()
                {
                    Name = "Asset"
                });

                dbContext.Roles.InsertOne(new Role()
                {
                    Name = "Redwan"
                });
            }
        }
    }

}

