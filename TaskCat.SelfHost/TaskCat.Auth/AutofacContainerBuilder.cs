namespace TaskCat.Auth
{
    using Autofac;
    using Data.Entity.Identity;
    using Lib;
    using Lib.Db;
    using Microsoft.AspNet.Identity;
    using Owin;
    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            #region Account

            builder.RegisterType<DbContext>().As<IDbContext>().SingleInstance();
            builder.RegisterType<AccountStore>().As<IUserStore<User>>().SingleInstance();
            builder.RegisterType<AccountManager>().SingleInstance();
            builder.RegisterType<AccountContext>().As<IAccountContext>().SingleInstance();
            builder.RegisterType<RoleManager>().SingleInstance();
            builder.RegisterType<ClientStore>().As<IClientStore>().SingleInstance();
            #endregion
        }
    }
}