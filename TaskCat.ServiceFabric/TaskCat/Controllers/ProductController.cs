namespace TaskCat.Controllers
{
    using Data.Entity;
    using Lib.Catalog;
    using Lib.Constants;
    using Lib.Domain;
    using Lib.Utility;
    using Lib.Utility.Odata;
    using LinqToQuerystring;
    using Microsoft.AspNet.Identity;
    using Model.Pagination;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class ProductController : ApiController
    {
        private IRepository<Product> productService;
        private IRepository<Store> storeService;

        public ProductController(IRepository<Product> productService, IRepository<Store> storeService)
        {
            this.productService = productService;
            this.storeService = storeService;
        }

        [HttpGet]
        [Route("api/Product/odata")]
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

            IQueryable<Product> products = productService.Collection.AsQueryable();
            var queryResult = products.LinqToQuerystring(queryString: odataQuery)
                .Skip(page * pageSize)
                .Take(pageSize);

            if (envelope)
                return Ok(new PageEnvelope<Product>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Ok(queryResult);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Product Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var product = await productService.Get(id);
            return Ok(product);
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin, Enterprise")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Product value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var authorizedId = this.User.Identity.GetUserId();
            var store = await storeService.Get(value.StoreId);

            if (!User.IsAdmin())
            {
                if (authorizedId != store.EnterpriseUserId)
                    throw new InvalidOperationException($"User {authorizedId} is not authorized to enlist a product in {store.Name}");
            }

            var result = await productService.Insert(value);
            return Content(HttpStatusCode.Created, result, new JsonMediaTypeFormatter());
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin, Enterprise")]
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody]Product value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var authorizedId = this.User.Identity.GetUserId();
            var store = await storeService.Get(value.StoreId);

            if (!User.IsAdmin())
            {
                if (authorizedId != store.EnterpriseUserId)
                    throw new InvalidOperationException($"User {authorizedId} is not authorized to update a product in {store.Name}");
            }

            var result = await productService.Update(value);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin, Enterprise")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(
             [Required(AllowEmptyStrings = false, ErrorMessage = "Product Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var authorizedId = this.User.Identity.GetUserId();
            var product = await this.productService.Get(id);
            var store = await storeService.Get(product.StoreId);

            if (!User.IsAdmin())
            {
                if (authorizedId != store.EnterpriseUserId)
                    throw new InvalidOperationException($"User {authorizedId} is not authorized to delete a product in {store.Name}");
            }

            var result = await productService.Delete(id);
            return Ok(result);
        }
    }
}
