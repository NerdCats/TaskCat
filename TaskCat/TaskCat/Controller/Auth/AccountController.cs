namespace TaskCat.Controller.Auth
{
    using Data.Model;
    using Data.Model.Identity;
    using Data.Model.Identity.Profile;
    using Data.Model.Identity.Registration;
    using Data.Model.Identity.Response;
    using Lib.Auth;
    using Lib.Constants;
    using Lib.Utility.Odata;
    using LinqToQuerystring;
    using Microsoft.AspNet.Identity;
    using Model.Pagination;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Lib.Utility;
    using Model.Account;
    /// <summary>
    /// Account (User And Asset related Controller)
    /// </summary>
    /// 

    [RoutePrefix("api/Account")] 
    public class AccountController : ApiController
    {
        private readonly AccountContext accountContext = null;

        /// <summary>
        /// Account Controller Constructor
        /// </summary>
        /// <param name="authRepository">
        /// AuthRepository is an Authentication Repository Instance
        /// </param>
        public AccountController(AccountContext authRepository)
        {
            this.accountContext = authRepository;
        }

        /// <summary>
        /// Registers an User or Asset in the system
        /// </summary>
        /// <param name="userModel">
        /// UserModel or AssetModel to register into system
        /// </param>
        /// <returns></returns>
        /// 

        [ResponseType(typeof(UserModel))]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegistrationModelBase userModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await accountContext.RegisterUser(userModel);

            if (!result.Result.Succeeded)
            {
                return GetErrorResult(result.Result);
            }

            await accountContext.NotifyUserCreationByMail(result.User, this.Request);

            return Created<UserModel>(Url.Link(AppConstants.GetUserProfileByIdRoute, new { userId = result.User.Id }), result.User.ToModel(isUserAuthenticated: false));
        }

        /// <summary>
        /// Route to confirm email address after a registration
        /// </summary>
        /// <param name="userId">
        /// userId created against the email
        /// </param>
        /// <param name="code">
        /// email verficiation token
        /// </param>
        /// <returns></returns>
        /// 
        [ResponseType(typeof(IdentityResult))]
        [HttpGet]
        [Route("ConfirmEmail", Name = AppConstants.ConfirmEmailRoute)]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await accountContext.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return GetErrorResult(result);
            }
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
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
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }


        /// <summary>
        /// View Public Profile on any user or asset
        /// </summary>
        /// <param name="userId">
        /// User Id for user or asset
        /// </param>
        /// <returns>
        /// Public profile for given user id, some properties are masked if you log in as anonymous as User or Asset
        /// </returns>
        /// 
        [AllowAnonymous]
        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [Route("Profile/{userId?}", Name = AppConstants.GetUserProfileByIdRoute)]
        [HttpGet]
        public async Task<IHttpActionResult> Profile(string userId = null)
        {
            if (!this.User.Identity.IsAuthenticated && string.IsNullOrEmpty(userId))
                return BadRequest("To get a public profile, please provide a valid user Id");

            if (string.IsNullOrWhiteSpace(userId))
                userId = this.User.Identity.GetUserId();

            var userModel = await accountContext.FindUserAsModel(userId);
            if (userModel == null) return NotFound();

            if (this.User.Identity.IsAuthenticated)
            {
                if (this.User.IsInRole("Administrator") || this.User.IsInRole("BackOfficeAdmin"))
                    userModel.IsUserAuthenticated = true;
                else if ((this.User.IsInRole("User") || this.User.IsInRole("Asset")) && (this.User.Identity.GetUserId() == userId))
                    userModel.IsUserAuthenticated = true;
            }

            return Json(userModel);
        }

        /// <summary>
        /// Gets Assigned Jobs to an Asset sorted by CreatedTime
        /// </summary>
        /// <param name="userId">
        /// userId for the Asset to find assigned jobs, would only work for Administrator and BackendAdministrator roles
        /// </param>
        /// <param name="pageSize">
        /// PageSize of the request, default is 10
        /// </param>
        /// <param name="page">
        /// Desired page number
        /// </param>
        /// <param name="dateTimeUpto">
        /// Results should be fetched from this date, usually results for last 5 days are sent back
        /// </param>
        /// <param name="jobStateUpto">
        /// Highest Job State to be fetched, default is IN_PROGRESS, that means by default ENQUEUED and IN_PROGRESS jobs would be fetched
        /// </param>
        /// <param name="sortDirection">
        /// default sort by CreatedTime direction, usually set at descending
        /// </param>
        /// <returns>
        /// A paginated set of results for Assigned job against an user id
        /// </returns>

        [Authorize(Roles = "Administrator, BackOfficeAdmin, Asset")]
        [HttpGet]
        [Route("{userId?}/jobs")]
        public async Task<IHttpActionResult> GetAssignedJobs(string userId = null, int pageSize = AppConstants.DefaultPageSize, int page = 0, DateTime? dateTimeUpto = null, JobState jobStateUpto = JobState.IN_PROGRESS, SortDirection sortDirection = SortDirection.Descending)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                if (this.User.IsInRole("Asset") && (this.User.Identity.GetUserId() != userId))
                    return Content(HttpStatusCode.Forbidden, "Accessing Assigned jobs of other Assets is not supported unless you're an admin");
            }
            else
            {
                userId = this.User.Identity.GetUserId();
            }

            var result = await accountContext.FindAssignedJobs(userId, page, pageSize, dateTimeUpto, jobStateUpto, sortDirection, this.Request);
            return Json(result);
        }


        /// <summary>
        /// Get All Users
        /// </summary>
        /// <param name="pageSize">
        /// Page size for the result, default is 10
        /// </param>
        /// <param name="page">
        /// Page number of the result
        /// </param>
        /// <param name="envelope">
        /// enveope in paged result, default is false, when enveloped, usually provide page number ,total and next and previous page links
        /// </param>
        /// <returns></returns>
        /// 

        [HttpGet]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("")]
        public async Task<IHttpActionResult> GetAllPaged(int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = false)
        {
            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize = 25;

            if (envelope)
                return Json(await accountContext.FindAllEnvelopedAsModel(page, pageSize, this.Request));
            else
                return Json(await accountContext.FindAllAsModel(page, pageSize));
        }

        /// <summary>
        /// Odata powered query to get users
        /// </summary>
        /// <remarks>
        /// It would basically be a collection where all the odata queries are done with standard TaskCat Paging
        /// Supported Odata query are $count, $filter, $orderBy, $skip, $top  
        /// </remarks>
        /// <param name="pageSize">
        /// pageSize for a single page
        /// </param>
        /// <param name="page">
        /// page number to be fetched
        /// </param>
        /// <param name="envelope">
        /// By default this is true, given false, the result comes as not paged
        /// </param>
        /// <returns>
        /// A list of Users And Assets that complies with the query
        /// </returns>
        /// 
        [ResponseType(typeof(UserModel))]
        [HttpGet]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("odata")]
        public async Task<IHttpActionResult> GetAllOdata(int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
        {
            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize = 25;

            var queryParams = this.Request.GetQueryNameValuePairs();
            queryParams.VerifyQuery(new List<string>() {
                    OdataOptionExceptions.InlineCount,
                    OdataOptionExceptions.Skip,
                    OdataOptionExceptions.Top
                });

            var odataQuery = queryParams.GetOdataQuery(new List<string>() {
                    "pageSize",
                    "page",
                    "envelope"
                });

            var users = await accountContext.FindAllAsModel();
            var queryResult = users.LinqToQuerystring(odataQuery).Skip(page * pageSize).Take(pageSize);

            if (envelope)
                return Json(new PageEnvelope<UserModel>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Json(queryResult);
        }

        /// <summary>
        /// profile
        /// </summary>
        /// <param name="model">
        /// Model 
        /// </param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [HttpPut]
        [Route("profile")]
        public async Task<IHttpActionResult> Update(IdentityProfile model)
        {
            return Json(await accountContext.Update(model, this.User.Identity.Name));
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPut]
        [Route("profile/{id}")]
        public async Task<IHttpActionResult> Update([FromBody]IdentityProfile model, [FromUri]string id)
        {
            return Json(await accountContext.UpdateById(model, id));
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [HttpPut]
        [Route("password")]
        public async Task<IHttpActionResult> UpdatePassword(PasswordUpdateModel model)
        {
            return Json(await accountContext.UpdatePassword(model, this.User.Identity.Name));
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [HttpPut]
        [Route("username")]
        public async Task<IHttpActionResult> UpdateUsername([FromUri]string newUsername)
        {
            return Json(await accountContext.UpdateUsername(newUsername, this.User.Identity.Name));
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPut]
        [Route("username/{userId}")]
        public async Task<IHttpActionResult> UpdateUsername([FromUri]string newUsername, string userId)
        {
            return Json(await accountContext.UpdateUsernameById(userId, newUsername));
        }

        // FIXME: Im not sure how this would pan out though, may be would pan out fine or not
        // Should I use HTTP Verbs here to determine result?
        /// <summary>
        /// Get whether a suggested username is availalble or not
        /// </summary>
        /// <param name="suggestedUserName">
        /// suggested username 
        /// </param>
        /// <returns>
        /// returns the availability of suggested user name or not
        /// </returns>
        [HttpGet]
        [Route("username")]
        [ResponseType(typeof(UsernameAvailibilityResponse))]
        public async Task<IHttpActionResult> SuggestUserName(string suggestedUserName)
        {
            var result = await accountContext.IsUsernameAvailable(suggestedUserName);
            return Json(new UsernameAvailibilityResponse(suggestedUserName, result));
        }

        [HttpPut]
        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [Route("contacts")]
        public async Task<IHttpActionResult> UpdateContacts(ContactUpdateModel model)
        {
            return Json(await accountContext.UpdateContacts(model, this.User.Identity.Name));
        }

        /// <summary>
        /// Upload an avatar/profile picture for an account
        /// </summary>
        /// <returns>
        /// return a FileUploadModel 
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Authorize(Roles = "User, Asset")]
        [Route("avatar")]
        public async Task<IHttpActionResult> UploadAvatar()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            var result = await accountContext.UploadAvatar(Request.Content, this.User.Identity.GetUserId());

            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest();
        }
    }
}
