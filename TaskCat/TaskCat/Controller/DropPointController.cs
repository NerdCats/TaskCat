namespace TaskCat.Controller
{
    using Data.Entity;
    using Data.Entity.Identity;
    using Lib.Constants;
    using Lib.DropPoint;
    using Lib.Utility.Odata;
    using LinqToQuerystring;
    using Microsoft.AspNet.Identity;
    using Model.Pagination;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Registers and manages drop points for a certain user
    /// </summary>
    public class DropPointController : ApiController
    {
        private IDropPointService service;

        public DropPointController(IDropPointService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("api/DropPoint/suggestions")]
        public IHttpActionResult GetDropPointNameSuggestions()
        {
            return Json(DropPointNameSuggestions.Values);
        }

        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> Get(string query, string userId = null, int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            var currentUserId = this.User.Identity.GetUserId();

            if (userId != null && currentUserId != userId
                && (!this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR) || !this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)))
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(userId))
                userId = currentUserId;

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize;
            var queryResult = await service.SearchDropPoints(userId, query);

            if (envelope)
                return Json(new PageEnvelope<DropPoint>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Json(queryResult);
        }

        [HttpGet]
        [Route("api/DropPoint/odata")]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        public async Task<IHttpActionResult> GetOdata(int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
        {
            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize;

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

            IQueryable<DropPoint> dropPoints = service.Collection.AsQueryable();
            var queryResult = dropPoints.LinqToQuerystring(queryString: odataQuery)
                .Skip(page * pageSize)
                .Take(pageSize);

            if (envelope)
                return Json(new PageEnvelope<DropPoint>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Json(queryResult);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> Post([FromBody]DropPoint value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorizedId = User.Identity.GetUserId();


            if (value.UserId != null && value.UserId != authorizedId
                && (!this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR) || !this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)))
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }

            value.Id = default(string);
            if (string.IsNullOrWhiteSpace(value.UserId))
                value.UserId = authorizedId;
            var result = await service.Insert(value);
            return Json(result);
        }

        [HttpPut]
        [Authorize]
        public async Task<IHttpActionResult> Put(DropPoint value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorizedId = User.Identity.GetUserId();

            if (value.UserId != null && value.UserId != authorizedId
                && (!this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR) || !this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)))
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }
         
            if(this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR) || this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN))
            {
                return Json(await service.Update(value));
            }
            else
            {
                return Json(await service.Update(value, value.UserId));
            }
            
        }

        [HttpDelete]
        [Authorize]
        public async Task<IHttpActionResult> Delete(string id, string userId = null)
        {
            var authorizedId = User.Identity.GetUserId();
            if (userId != null && userId != authorizedId
                && (!this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR) || !this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN)))
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(userId))
                userId = authorizedId;

            if (this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR) || this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN))
            {
                return Json(await service.Delete(id));
            }
            else
            {
                return Json(await service.Delete(id, userId));
            }
        }
    }
}
