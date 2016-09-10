namespace TaskCat.Controller
{
    using Microsoft.AspNet.Identity;
    using System.Collections.Generic;
    using System.Web.Http;
    using Data.Entity;
    using Lib.Utility;
    using System;
    using System.Threading.Tasks;
    using System.Net.Http.Formatting;
    using System.Net;
    using Lib.Constants;
    using System.ComponentModel.DataAnnotations;
    using System.Net.Http;
    using Lib.Utility.Odata;
    using System.Linq;
    using Model.Pagination;
    using LinqToQuerystring;
    using MongoDB.Driver;
    using Lib.Domain;

    public class StoreController : ApiController
    {
        private IRepository<Store> service;

        public StoreController(IRepository<Store> service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("api/ProductCategory/odata")]
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
                return Json(new PageEnvelope<Store>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Json(queryResult);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Store Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var store = await service.Get(id);
            return Json(store);
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
            return Json(result);
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
            return Json(result);
        }
    }
}
