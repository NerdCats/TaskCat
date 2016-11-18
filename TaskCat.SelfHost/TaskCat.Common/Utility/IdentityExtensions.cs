namespace TaskCat.Common.Lib.Utility
{
    using Data.Entity.Identity;
    using System.Security.Principal;

    /// <summary>
    /// Extension methods to help with Identity entities
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        ///     Returns whether an user is an admin
        ///     backend office admin or not
        /// </summary>
        /// <param name="user">
        ///     Identity user that is to be checked
        /// </param>
        /// <returns></returns>
        public static bool IsAdmin(this IPrincipal user)
        {
            return user.IsInRole(RoleNames.ROLE_ADMINISTRATOR)
                || user.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN);
        }
    }
}