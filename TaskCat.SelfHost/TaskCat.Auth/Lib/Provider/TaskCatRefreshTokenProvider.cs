namespace TaskCat.Auth.Lib.Provider
{
    using Microsoft.Owin.Security.Infrastructure;
    using System;
    using System.Threading.Tasks;
    using Data.Entity.Identity;
    using Common.Utility;
    using Core;

    public class TaskCatRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly IAccountContext authRepository;
        private const string clientPropertyKey = "audience";

        public TaskCatRefreshTokenProvider(IAccountContext authRepository)
        {
            this.authRepository = authRepository;
        }
       
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary[clientPropertyKey];
            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshToken()
            {
                Id = HashMaker.GetHash(refreshTokenId),
                ClientId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            var result = await authRepository.AddRefreshToken(token);

            if (result)
            {
                context.SetToken(refreshTokenId);
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var hashedTokenId = HashMaker.GetHash(context.Token);

            var refreshToken = await authRepository.FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                // Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);

                var result = await authRepository.RemoveRefreshToken(hashedTokenId);
            }
        }


        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException("Try CreateAsync please, Sync stuff is gone the way of the dodo bird my friend");
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException("Try CreateAsync please, Sync stuff is gone the way of the dodo bird my friend");
        }
    }
}