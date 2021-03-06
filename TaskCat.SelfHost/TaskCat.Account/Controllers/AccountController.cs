﻿namespace TaskCat.Account.Controllers
{
    using Data.Model;
    using Data.Model.Identity;
    using Data.Model.Identity.Profile;
    using Data.Model.Identity.Registration;
    using Data.Model.Identity.Response;
    using Lib.Constants;
    using Microsoft.AspNet.Identity;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Net.Http.Formatting;
    using Data.Entity.Identity;
    using Common.Utility.ActionFilter;
    using Common.Utility.Odata;
    using Common.Email;
    using Common.Model.Response;
    using Core.Model.Account;
    using Core;
    using Core.Utility;
    using AppSettings = Its.Configuration.Settings;
    using Settings;
    using Common.Utility;
    using Common.Settings;

    /// <summary>
    /// Account (User And Asset related Controller)
    /// </summary>
    /// 
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly IAccountContext accountContext = null;

        /// <summary>
        /// Account Controller Constructor
        /// </summary>
        /// <param name="accountContext">
        /// AuthRepository is an Authentication Repository Instance
        /// </param>
        public AccountController(IAccountContext accountContext)
        {
            this.accountContext = accountContext;
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

            var clientSettings = AppSettings.Get<ClientSettings>();
            clientSettings.Validate();

            await accountContext.NotifyUserCreationByMail(result.User, clientSettings.WebCatUrl, clientSettings.ConfirmEmailPath, EmailTemplatesConfig.WelcomeEmailTemplate);

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

        /// <summary>
        /// Resend Confirmation Mails from the system if needed
        /// </summary>
        /// <param name="userId">
        /// user that needs a confirmation mail 
        /// </param>
        /// <returns>
        /// Sends back a response that states the status of the email request
        /// </returns>
        [ResponseType(typeof(SendEmailResponse))]
        [HttpGet]
        [Route("ResendConfirmEmail")]
        public async Task<IHttpActionResult> ResendConfirmationEmail(string userId)
        {
            var user = await accountContext.FindUser(userId);
            if (user.EmailConfirmed)
            {
                return Content(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse()
                    {
                        Message = string.Concat("User with ", userId, " has already his email confirmed"),
                        Data = new { emailAlreadyConfirmed = true }
                    },
                    new JsonMediaTypeFormatter());
            }

            var clientSettings = AppSettings.Get<ClientSettings>();
            clientSettings.Validate();

            var result = await accountContext.NotifyUserCreationByMail(user, clientSettings.WebCatUrl, clientSettings.ConfirmEmailPath, EmailTemplatesConfig.WelcomeEmailTemplate);

            if (!result.Success)
                return Content(HttpStatusCode.InternalServerError, result, new JsonMediaTypeFormatter());
            else
                return Ok(result);
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
        [Authorize]
        [Route("Profile/{userId?}", Name = AppConstants.GetUserProfileByIdRoute)]
        [HttpGet]
        public async Task<IHttpActionResult> Profile(string userId = null)
        {
            if (this.User == null && string.IsNullOrEmpty(userId))
                return BadRequest("To get a public profile, please provide a valid user Id");

            if (string.IsNullOrWhiteSpace(userId))
                userId = this.User.Identity.GetUserId();

            var userModel = await accountContext.FindUserAsModel(userId);
            if (userModel == null) return NotFound();

            if (this.User!=null && this.User.Identity.IsAuthenticated)
            {
                if (this.User.IsInRole("Administrator") || this.User.IsInRole("BackOfficeAdmin"))
                    userModel.IsUserAuthenticated = true;
                else if ((this.User.IsInRole("User") || this.User.IsInRole("Asset")) && (this.User.Identity.GetUserId() == userId))
                    userModel.IsUserAuthenticated = true;
            }

            return Ok(userModel);
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

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize;

            if (envelope)
                return Ok(await accountContext.FindAllEnvelopedAsModel(page, pageSize, this.Request, AppConstants.DefaultApiRoute));
            else
                return Ok(await accountContext.FindAllAsModel(page, pageSize));
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
        [Route("odata", Name = AppConstants.AccountOdataRoute)]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> GetAllOdata()
        {
            var users = await accountContext.FindAllAsModel();

            var odataResult = await users.ToOdataResponse(this.Request, AppConstants.AccountOdataRoute);
            return Ok(odataResult);
        }

        /// <summary>
        /// profile
        /// </summary>
        /// <param name="model">
        /// Model 
        /// </param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("profile")]
        public async Task<IHttpActionResult> Update(IdentityProfile model)
        {
            return Ok(await accountContext.Update(model, this.User.Identity.Name));
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPut]
        [Route("profile/{id}")]
        public async Task<IHttpActionResult> Update([FromBody]IdentityProfile model, [FromUri]string id)
        {
            return Ok(await accountContext.UpdateById(model, id));
        }

        [Authorize]
        [HttpPut]
        [Route("password")]
        public async Task<IHttpActionResult> UpdatePassword(PasswordUpdateModel model)
        {
            return Ok(await accountContext.UpdatePassword(model, this.User.Identity.Name));
        }

        [Authorize]
        [HttpPut]
        [Route("username")]
        public async Task<IHttpActionResult> UpdateUsername([FromUri]string newUsername)
        {
            return Ok(await accountContext.UpdateUsername(newUsername, this.User.Identity.Name));
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPut]
        [Route("username/{userId}")]
        public async Task<IHttpActionResult> UpdateUsername([FromUri]string newUsername, string userId)
        {
            return Ok(await accountContext.UpdateUsernameById(userId, newUsername));
        }

        /// <summary>
        /// Check availibility of a suggsted <see cref="CheckPropertyNames.EMAIL"/> , <see cref="CheckPropertyNames.PHONENUMBER"/> or an <see cref="CheckPropertyNames.EMAIL"/>
        /// </summary>
        /// <returns>
        /// returns whether the suggested value is available
        /// </returns>
        [HttpGet]
        [Route("check")]
        [ResponseType(typeof(AvailibilityResponse))]
        public async Task<IHttpActionResult> Check()
        {
            var queryParams = this.Request.GetQueryNameValuePairs();
            if (queryParams.Count() > 1)
                throw new NotSupportedException("More than one parameter availibility check is supported");

            var queryKeyVal = queryParams.First();
            var property = queryKeyVal.Key;
            var suggestedValue = queryKeyVal.Value;

            bool result;

            switch (property.ToLower())
            {
                case CheckPropertyNames.EMAIL:
                    result = await accountContext.IsEmailAvailable(suggestedValue);
                    break;
                case CheckPropertyNames.PHONENUMBER:
                    result = await accountContext.IsPhoneNumberAvailable(suggestedValue);
                    break;
                case CheckPropertyNames.USERNAME:
                    result = await accountContext.IsUsernameAvailable(suggestedValue);
                    break;
                default:
                    return Content(HttpStatusCode.BadRequest, new ErrorResponse()
                    {
                        Message = $"Availibility check on {property} is not supported, supported property names are listed in data",
                        Data = new List<string>() {
                            CheckPropertyNames.EMAIL,
                            CheckPropertyNames.PHONENUMBER,
                            CheckPropertyNames.USERNAME
                        }
                    }, new JsonMediaTypeFormatter());
            }

            return Ok(new AvailibilityResponse(property, suggestedValue, result));
        }

        [HttpPut]
        [Authorize(Roles = "Administrator, BackOfficeAdmin, User, Asset")]
        [Route("contacts")]
        public async Task<IHttpActionResult> UpdateContacts(ContactUpdateModel model)
        {
            return Ok(await accountContext.UpdateContacts(model, this.User.Identity.Name));
        }

        /// <summary>
        /// Upload an avatar/profile picture for an account
        /// </summary>
        /// <returns>
        /// return a FileUploadModel 
        /// </returns>
        [HttpPost]
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

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {
            Uri redirectUri;
            var queryStrings = request.GetQueryNameValuePairs();
            var redirectUriString = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, "redirect_uri", true) == 0).Value;

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, "client_id", true) == 0).Value;

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            // TODO: I need to find client here, no client repository to be honest
            Client client = null;

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }

    }
}
