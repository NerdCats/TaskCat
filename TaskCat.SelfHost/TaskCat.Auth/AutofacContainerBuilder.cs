namespace TaskCat.Auth
{
    using Autofac;
    using Owin;
    public class AutofacContainerBuilder
    {
        public IContainer BuildContainer(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            #region Account

            builder.RegisterType<DbContext>().As<IDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<AccountStore>().As<IUserStore<User>>().InstancePerLifetimeScope();
            builder.RegisterType<AccountManager>().InstancePerLifetimeScope();
            builder.RegisterType<AccountContext>().As<IAccountContext>().InstancePerLifetimeScope();
            builder.RegisterType<RoleManager>().InstancePerLifetimeScope();
            builder.RegisterType<ClientStore>().As<IClientStore>().SingleInstance();
            #endregion
        }
    }
}