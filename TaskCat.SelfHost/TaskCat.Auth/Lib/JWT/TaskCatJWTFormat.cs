namespace TaskCat.Auth.Lib.JWT
{
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.DataHandler.Encoder;
    using System;
    using Data.Entity.Identity;
    using Thinktecture.IdentityModel.Tokens;
    using System.IdentityModel.Tokens;
    using Account.Core;

    public class TaskCatJWTFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private const string clientPropertyKey = "audience";
        private IClientStore clientStore;
        private readonly string issuer = string.Empty;

        public TaskCatJWTFormat(string issuer, IClientStore clientStore)
        {
            this.issuer = issuer;
            this.clientStore = clientStore;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            string clientId = data.Properties.Dictionary.ContainsKey(clientPropertyKey) ? data.Properties.Dictionary[clientPropertyKey] : null;

            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");

            Client audience = clientStore.FindClient(clientId).GetAwaiter().GetResult();

            string symmetricKeyAsBase64 = audience.Secret;
            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            var signingKey = new HmacSigningCredentials(keyByteArray);

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityToken(issuer, clientId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}
