namespace TaskCat.Common.Lib.Utility
{
    using Data.Entity.Identity;
    using System.Security.Claims;
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

        /// <summary>
        ///     Returns whether an user is User or an Enterprise user
        /// </summary>
        /// <param name="user">
        ///     Identity user that is to be checked
        /// </param>
        /// <returns></returns>
        public static bool IsUserOrEnterpriseUserOnly(this IPrincipal user)
        {
            return ((user.IsInRole(RoleNames.ROLE_USER) || user.IsInRole(RoleNames.ROLE_ENTERPRISE))
                     && !user.IsInRole(RoleNames.ROLE_ADMINISTRATOR)
                     && !user.IsInRole(RoleNames.ROLE_ASSET)
                     && !user.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN));
        }

        /// <summary>
        ///     Get full name of a user if claimed
        /// </summary>
        /// <param name="user">
        ///     Identity user that is to be checked
        /// </param>
        /// <returns></returns>
        public static string GetUserFullName(this IPrincipal user)
        {
            var identity = user as ClaimsIdentity;
            var fullNameClaim = identity.FindFirst(x => x.Type == ClaimTypes.GivenName)?.Value;
            return fullNameClaim;
        }
    }
}