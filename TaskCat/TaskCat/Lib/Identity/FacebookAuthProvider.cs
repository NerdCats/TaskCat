namespace TaskCat.Lib.Identity
{
    using Microsoft.Owin.Security.Facebook;
    using System.Security.Claims;
    using System.Threading.Tasks;

    internal class FacebookAuthProvider : FacebookAuthenticationProvider
    {
        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            if (!context.Identity.HasClaim(ClaimTypes.Authentication, "true"))
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.Authentication, "true"));
            }
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            return Task.FromResult<object>(null);
        }
    }
}