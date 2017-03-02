namespace TaskCat.Warehouse
{
    using Autofac.Integration.WebApi;
    using NerdCats.Owin;
    using Owin;
    using System.Reflection;
    using System.Web.Http;
    using Autofac;
    using System;
    using Common.Settings;
    using AppSettings = Its.Configuration.Settings;
    using Account.Core;
    using System.Linq;
    using Microsoft.Owin.Security.Jwt;
    using Microsoft.Owin.Security.DataHandler.Encoder;
    using Microsoft.Owin.Security;
    using Common.Utility.ActionFilter;

    public static class Startup
    {
        public static void ConfigureApp(IAppBuilder app)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            AutofacContainerBuilder builder = new AutofacContainerBuilder();

            app.UseErrorPage();

            var container = builder.BuildContainer(app);
            app.UseAutofacMiddleware(container);
            app.UseForwardHeaders(options: default(ForwardedHeadersOptions));

            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(container);
            var config = new HttpConfiguration();
            config.EnableCors();

            ConfigureResourceOAuth(app, container);

            WebApiConfig.Register(config, webApiDependencyResolver);
            config.Filters.Add(new ErrorDocumentFilter());

            app.UseWebApi(config);
            app.UseAutofacWebApi(config);

            app.Run(context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync(string.Format($"Welcome to TaskCat Warehouse '{version}', proudly baked by NerdCats"));
            });
        }

        private static void ConfigureResourceOAuth(IAppBuilder app, IContainer container)
        {
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