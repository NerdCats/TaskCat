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
    using Data.Model.Identity.Registration;
    using Lib.Constants;
    using Data.Model.Identity.Response;
    using Data.Model.Identity.Profile;
    using System.Web.OData.Query;
    using Model.Pagination;
    using Data.Model;
    using MongoDB.Driver;

    /// <summary>
    /// Account (User And Asset related Controller)
    /// </summary>
    /// 

    [RoutePrefix("api/Account")] 
    public class AccountController : ApiController
    {
        private readonly AccountRepository accountRepository = null;

        /// <summary>
        /// Account Controller Constructor
        /// </summary>
        /// <param name="authRepository">
        /// AuthRepository is an Authentication Repository Instance
        /// </param>
        public AccountController(AccountRepository authRepository)
        {
            this.accountRepository = authRepository;
        }

        /// <summary>
        /// Registers an User or Asset in the system
        /// </summary>
        /// <param name="userModel">
        /// UserModel or AssetModel to register into system
        /// </param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegistrationModelBase userModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await accountRepository.RegisterUser(userModel);

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

        /// <summary>
        /// View Public Profile on any user or asset
        /// </summary>
        /// <param name="userId">
        /// User Id for user or asset
        /// </param>
        /// <returns>
        /// Public profile for given user id, some properties are masked if you log in as anonymous as User or Asset
        /// </returns>
        [AllowAnonymous]
        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [Route("Profile/{userId?}")]
        [HttpGet]
        public async Task<IHttpActionResult> Profile(string userId = null)
        {
            try
            {
                if (!this.User.Identity.IsAuthenticated && string.IsNullOrEmpty(userId))
                    return BadRequest("To get a public profile, please provide a valid user Id");

                if (string.IsNullOrWhiteSpace(userId))
                    userId = this.User.Identity.GetUserId();

                var userModel = await accountRepository.FindUserAsModel(userId);
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
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
        [Route("Jobs/{userId?}")]
        public async Task<IHttpActionResult> GetAssignedJobs(string userId = null, int pageSize = AppConstants.DefaultPageSize, int page = 0, DateTime? dateTimeUpto = null, JobState jobStateUpto = JobState.IN_PROGRESS, SortDirection sortDirection = SortDirection.Descending)
        {
            try
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

                var result = await accountRepository.FindAssignedJobs(userId, page, pageSize, dateTimeUpto, jobStateUpto, sortDirection, this.Request);
                return Json(result);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
        [HttpGet]
        [AllowAnonymous]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("")]
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
                    return Json(await accountRepository.FindAllEnvelopedAsModel(page, pageSize, this.Request));
                else
                    return Json(await accountRepository.FindAllAsModel(page, pageSize));
            }
            catch (Exception ex) { return InternalServerError(ex); }


        }

        /// <summary>
        /// Odata powered query to get users
        /// </summary>
        /// <param name="query">
        /// It would basically be a collection where all the odata queries are done with standard TaskCat Paging
        /// Supported Odata query are $count, $filter, $orderBy, $skip, $top  
        /// </param>
        /// <param name="pageSize">
        /// pageSize for a single page
        /// </param>
        /// <param name="page">
        /// page number to be fetched
        /// </param>
        /// <returns>
        /// A list of Users And Assets that complies with the query
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("odata")]
        public async Task<IHttpActionResult> GetAll(ODataQueryOptions<UserModel> query, int pageSize = AppConstants.DefaultPageSize, int page = 0)
        {
            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize = 25;

            try
            {
                var settings = new ODataValidationSettings()
                {
                    // Initialize settings as needed.
                    AllowedFunctions = AllowedFunctions.AllMathFunctions,
                    AllowedQueryOptions=AllowedQueryOptions.Count|AllowedQueryOptions.Filter|AllowedQueryOptions.OrderBy|AllowedQueryOptions.Skip|AllowedQueryOptions.Top
                };

                query.Validate(settings);

                var users = await accountRepository.FindAllAsModelAsQueryable(page, pageSize);
                
                var queryResult = (query.ApplyTo(users)) as IEnumerable<UserModel>;
                if (query.Count.Value)
                    return Json(new PageEnvelope<UserModel>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
                return Json(queryResult);
            }
            catch (Exception ex) { return InternalServerError(ex); }
        }

        /// <summary>
        /// profile
        /// </summary>
        /// <param name="model">
        /// 
        /// </param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [HttpPut]
        [Route("profile")]
        public async Task<IHttpActionResult> Update(IdentityProfile model)
        {
            try
            {
                return Json(await accountRepository.Update(model, this.User.Identity.Name));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPut]
        [Route("profile/{id}")]
        public async Task<IHttpActionResult> Update( [FromBody]IdentityProfile model, [FromUri]string id)
        {
            try
            {
                return Json(await accountRepository.UpdateById(model, id));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [HttpPut]
        [Route("password")]
        public async Task<IHttpActionResult> UpdatePassword(PasswordUpdateModel model)
        {
            try
            {
                return Json(await accountRepository.UpdatePassword(model, this.User.Identity.Name));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [HttpPut]
        [Route("username")]
        public async Task<IHttpActionResult> UpdateUsername([FromUri]string newUsername)
        {
            try
            {
                return Json(await accountRepository.UpdateUsername(newUsername, this.User.Identity.Name));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPut]
        [Route("username/{userId}")]
        public async Task<IHttpActionResult> UpdateUsername([FromUri]string newUsername, string userId)
        {
            try
            {
                return Json(await accountRepository.UpdateUsernameById(userId, newUsername));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // FIXME: Im not sure how this would pan out though, may be would pan out fine or not
        // Should I use HTTP Verbs here to determine result?
        [HttpGet]
        [Route("username")]
        public async Task<IHttpActionResult> SuggestUserName(string suggestedUserName)
        {
            try
            {
                return Json(await accountRepository.IsUsernameAvailable(suggestedUserName));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [Route("contacts")]
        public async Task<IHttpActionResult> UpdateContacts(ContactUpdateModel model)
        {
            try
            {
                return Json(await accountRepository.UpdateContacts(model, this.User.Identity.Name));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
            try
            {
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }
                
                var result = await accountRepository.UploadAvatar(Request.Content, this.User.Identity.GetUserId());

                if (result != null)
                {
                    return Ok(result);
                }

                return BadRequest();

            }
            catch(NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
