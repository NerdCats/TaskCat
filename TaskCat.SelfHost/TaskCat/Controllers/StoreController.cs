using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using LinqToQuerystring;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using TaskCat.Data.Entity;
using TaskCat.Lib.Constants;
using TaskCat.Lib.Domain;
using TaskCat.Lib.Utility;
using TaskCat.Lib.Utility.Odata;
using TaskCat.Model.Pagination;

namespace TaskCat.Controllers
{
    public class StoreController : ApiController
    {
        private IRepository<Store> service;

        public StoreController(IRepository<Store> service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("api/Store/odata")]
        public async Task<IHttpActionResult> Get(int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
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

            IQueryable<Store> productCategories = service.Collection.AsQueryable();
            var queryResult = productCategories.LinqToQuerystring(queryString: odataQuery)
                .Skip(page * pageSize)
                .Take(pageSize);

            if (envelope)
                return Ok(new PageEnvelope<Store>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Ok(queryResult);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Store Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var store = await service.Get(id);
            return Ok(store);
        }

        [Authorize(Roles = "Administrator, Enterprise, BackOcffficeAdmin")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Store store)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var authorizedId = this.User.Identity.GetUserId();
            if (!User.IsAdmin())
            {
                if (authorizedId != store.EnterpriseUserId)
                    throw new InvalidOperationException($"User {authorizedId} is not authorized to create a store for user {store.EnterpriseUserId}");

                store.DisplayOrder = AppConstants.DefaultStoreOrder;
            }

            var result = await service.Insert(store);
            return Content(HttpStatusCode.Created, store, new JsonMediaTypeFormatter());
        }

        [Authorize(Roles = "Administrator, Enterprise, BackOfficeAdmin")]
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody]Store store)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var authorizedId = this.User.Identity.GetUserId();
            if (!User.IsAdmin())
            {
                if (authorizedId != store.EnterpriseUserId)
                    throw new InvalidOperationException($"User {authorizedId} is not authorized to update a store for user {store.EnterpriseUserId}");

                store.DisplayOrder = AppConstants.DefaultStoreOrder;
            }

            var result = await service.Update(store);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator, Enterprise, BackOfficeAdmin")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Store Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var authorizedId = this.User.Identity.GetUserId();
            if (!User.IsAdmin())
            {
                var jobUserId = (await service.Get(id)).EnterpriseUserId;
                if (authorizedId != jobUserId)
                    throw new InvalidOperationException($"User {authorizedId} is not authorized to delete a store for user {jobUserId}");                
            }

            var result = await service.Delete(id);
            return Ok(result);
        }
    }
}
