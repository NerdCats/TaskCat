namespace TaskCat.Controller
{
    using Data.Entity;
    using Lib.Catalog;
    using Lib.Constants;
    using Lib.Utility.Odata;
    using LinqToQuerystring;
    using Model.Pagination;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using System.Web.Http;

    [Authorize(Roles = "Administrator, BackOfficeAdmin")]
    public class ProductCategoryController : ApiController
    {
        private IProductCategoryService service;

        public ProductCategoryController(IProductCategoryService service)
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

            IQueryable<ProductCategory> productCategories = service.Collection.AsQueryable();
            var queryResult = productCategories.LinqToQuerystring(queryString: odataQuery)
                .Skip(page * pageSize)
                .Take(pageSize);

            if (envelope)
                return Json(new PageEnvelope<ProductCategory>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Json(queryResult);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Product Category Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await service.Get(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]ProductCategory category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await service.Insert(category);
            return Content(HttpStatusCode.Created, result, new JsonMediaTypeFormatter());
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody]ProductCategory category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(category.Id))
                return BadRequest("Product Category Id not provided");

            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await service.Update(category);
            return Json(result);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Product Category Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await service.Delete(id);
            return Json(result);
        }
    }
}
