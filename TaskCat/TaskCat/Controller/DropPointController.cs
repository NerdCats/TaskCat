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
    using System.Web.Http.Description;

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

        /// <summary>
        /// Gets a Drop Point based on id and userId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId">
        /// Optional param, needed if an Administrator is actually looking for someone else's DropPoint
        /// </param>
        /// <returns>
        /// A DropPoint matching the query
        /// </returns>
        [HttpGet]
        [Authorize]
        [ResponseType(typeof(DropPoint))]
        public async Task<IHttpActionResult> Get(string id, string userId=null)
        {
            var authorizedId = User.Identity.GetUserId();
            if (userId != null && userId != authorizedId
                && !IsUserAdminOrBackendOfficeAdmin())
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(userId))
                userId = authorizedId;

            if (IsUserAdminOrBackendOfficeAdmin())
            {
                return Ok(await service.Get(id));
            }
            else
            {
                return Ok(await service.Get(id, userId));
            }
        }

        /// <summary>
        /// Get a list of predefined name suggestions for the drop points
        /// </summary>
        /// <returns>
        /// List of predefined name suggestions for the drop points
        /// </returns>
        /// 
        [HttpGet]
        [Route("api/DropPoint/suggestions")]
        [ResponseType(typeof(List<string>))]
        public IHttpActionResult GetDropPointNameSuggestions()
        {
            return Ok(DropPointNameSuggestions.Values);
        }

        /// <summary>
        /// Gets a list of drop points based on a search on Address and Name 
        /// </summary>
        /// <param name="query">
        /// Text to search in the Address and Name in a Drop Point
        /// </param>
        /// <param name="userId">
        /// Optional, this field is populated automatically from authentication header if not provided
        /// If an administrator wants to search in one specific user's droppoint list he would have to use 
        /// this field
        /// </param>
        /// <param name="pageSize">
        /// page size to return result in
        /// </param>
        /// <param name="page">
        /// page number 
        /// </param>
        /// <param name="envelope">
        /// Defines whether the result should be sent back as a paged result or not
        /// </param>
        /// <returns>
        /// Returns either a <see cref="PageEnvelope{DropPoint}"/> or a <see cref="List{DropPoint}"/>
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("api/DropPoint/search")]
        public async Task<IHttpActionResult> Search(string query, string userId = null, int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query));
            }

            var currentUserId = this.User.Identity.GetUserId();

            if (userId != null && currentUserId != userId
                && !IsUserAdminOrBackendOfficeAdmin())
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(userId))
                userId = currentUserId;

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize;
            var queryResult = await service.SearchDropPoints(userId, query);

            if (envelope)
                return Ok(new PageEnvelope<DropPoint>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Ok(queryResult);
        }

        /// <summary>
        /// Odata endpoint for only Administrator and BackendOfficeAdmins to execute queries needed 
        /// for components that needs a comprehensive list
        /// </summary>
        /// <param name="pageSize">
        /// page size
        /// </param>
        /// <param name="page">
        /// page number
        /// </param>
        /// <param name="envelope">
        /// Defines whether the result should be sent back as a paged result or not
        /// </param>
        /// <returns>
        /// Returns either a <see cref="PageEnvelope{DropPoint}"/> or a <see cref="List{DropPoint}"/>
        /// </returns>
        /// 
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
                return Ok(new PageEnvelope<DropPoint>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Ok(queryResult);
        }

        /// <summary>
        /// Submit a new drop point 
        /// </summary>
        /// <param name="value">
        /// DropPoint Value to be submitted
        /// </param>
        /// <returns>
        /// The newly created drop point
        /// </returns>
        [HttpPost]
        [Authorize]
        [ResponseType(typeof(DropPoint))]
        public async Task<IHttpActionResult> Post([FromBody]DropPoint value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorizedId = User.Identity.GetUserId();


            if (value.UserId != null && value.UserId != authorizedId
                && !IsUserAdminOrBackendOfficeAdmin())
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }

            value.Id = default(string);
            if (string.IsNullOrWhiteSpace(value.UserId))
                value.UserId = authorizedId;
            var result = await service.Insert(value);
            return Ok(result);
        }

        private bool IsUserAdminOrBackendOfficeAdmin()
        {
            return (this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR) || this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN));
        }

        /// <summary>
        /// Update a certain drop point
        /// </summary>
        /// <param name="value">
        /// Drop point to be updated, only an administrator can save change any
        /// users drop point userId field to other userId field
        /// </param>
        /// <returns>
        /// Returns the modified drop point
        /// </returns>
        [HttpPut]
        [Authorize]
        [ResponseType(typeof(DropPoint))]
        public async Task<IHttpActionResult> Put(DropPoint value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorizedId = User.Identity.GetUserId();

            if (value.UserId != null && value.UserId != authorizedId
                && !IsUserAdminOrBackendOfficeAdmin())
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }
         
            if(IsUserAdminOrBackendOfficeAdmin())
            {
                return Ok(await service.Update(value));
            }
            else
            {
                return Ok(await service.Update(value, value.UserId));
            }
            
        }

        /// <summary>
        /// Deletes a drop point based on id and optionally user id
        /// </summary>
        /// <param name="id">
        /// Sepcific drop point id to be deleted
        /// </param>
        /// <param name="userId">
        /// Optional, if an Administrator wants to delete a specific id of an user, he can use this field
        /// </param>
        /// <returns>
        /// Deleted drop point
        /// </returns>
        [HttpDelete]
        [Authorize]
        [ResponseType(typeof(DropPoint))]
        public async Task<IHttpActionResult> Delete(string id, string userId = null)
        {
            var authorizedId = User.Identity.GetUserId();
            if (userId != null && userId != authorizedId
                && !IsUserAdminOrBackendOfficeAdmin())
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(userId))
                userId = authorizedId;

            if (IsUserAdminOrBackendOfficeAdmin())
            {
                return Ok(await service.Delete(id));
            }
            else
            {
                return Ok(await service.Delete(id, userId));
            }
        }
    }
}
