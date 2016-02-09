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
    using Lib.Constants;
    using Data.Model.Identity.Response;
    using Data.Model.Identity.Profile;

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
                    return BadRequest();               
                    // No ModelState errors are available to send, 
                    // so just return an empty BadRequest.

                return BadRequest(ModelState);
            }

            return null;
        }

        [AllowAnonymous]
        [Authorize(Roles= "Administrator, BackOfficeAdmin, User")]
        [Route("{userId}")]
        public async Task<IHttpActionResult> Get(string userId)
        {
            var userModel = await authRepository.FindUserAsModel(userId);
            if (userModel == null) return NotFound();

            userModel.IsUserAuthenticated = this.User.IsInRole("Administrator");        
            return Json(userModel);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        public async Task<IHttpActionResult> GetAllPaged(int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = false)
        {
            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize = 25;

            try
            {
                if (envelope)
                    return Json(await authRepository.FindAllEnvelopedAsModel(page, pageSize, this.Request));
                else
                    return Json(await authRepository.FindAllAsModel(page, pageSize));
            }
            catch (Exception ex) { return InternalServerError(ex); }
                
            
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin, User")]
        [HttpPut]
        [Route("profile")]
        public async Task<IHttpActionResult> Update(UserProfile model)
        {
            try
            {
                return Json(await authRepository.Update(model, this.User.Identity.Name));
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
