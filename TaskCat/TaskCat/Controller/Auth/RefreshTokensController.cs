namespace TaskCat.Controller.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Lib.Auth;
    using System.Threading.Tasks;

    [RoutePrefix("api/RefreshTokens")]
    public class RefreshTokensController : ApiController
    {
        private readonly AccountContext authRepository = null;

        public RefreshTokensController(AccountContext authRepository)
        {
            this.authRepository = authRepository;
        }

        [Authorize(Users = "Admin")]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(authRepository.GetAllRefreshTokens());
        }

        // FIXME: Would this be needed? Um.. not sure
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> Delete(string tokenId)
        {
            if (await authRepository.RemoveRefreshToken(tokenId))
                return Ok();

            return BadRequest("Token Id does not exist");
        }
    }
}
