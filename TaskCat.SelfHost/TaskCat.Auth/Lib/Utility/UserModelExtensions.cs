namespace TaskCat.Auth.Lib.Utility
{
    using Data.Model.Identity.Response;
    using Data.Entity.Identity;
    using Data.Model.Identity;

    internal static class UserModelExtensions
    {
        public static UserModel ToModel(this User user, bool isUserAuthenticated)
        {
            switch (user.Type)
            {
                case IdentityTypes.USER:
                    return new UserModel(user as User, isUserAuthenticated);
                case IdentityTypes.ENTERPRISE:
                    return new EnterpriseUserModel(user as EnterpriseUser, isUserAuthenticated);
                default:
                    return new AssetModel(user as Asset, isUserAuthenticated);
            }
        }
    }
}