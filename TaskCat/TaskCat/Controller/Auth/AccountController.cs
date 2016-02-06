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
    using Data.Model.Identity;
    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using Lib.Utility.Converter;
    using Data.Model.Identity.Registration;

    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly AuthRepository authRepository = null;

        public AccountController(AuthRepository authRepository)
        {
            this.authRepository = authRepository;
        }

        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserRegistrationModel userModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await authRepository.RegisterUser(userModel);

                var errorResult = GetErrorResult(result);

                if (errorResult != null)
                {
                    return errorResult;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                //FIXME: Need to log these
                return InternalServerError(ex);
            }
        }

        // FIXME: this definitely looks ugly, need to clean up here
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, \
                    // so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        [Authorize(Roles="Administrator, User")]
        [Route("{userId}")]
        public async Task<IHttpActionResult> Get(string userId)
        {
            if (this.User.IsInRole("Administrator"))
            {
                return Json(await authRepository.FindUser(userId));
            }
            else
            {
                throw new NotImplementedException();
            }

            
        }


    }
}
