using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using LinqToQuerystring;
using MongoDB.Driver;
using TaskCat.Data.Entity;
using TaskCat.Lib.Constants;
using TaskCat.Lib.Domain;
using TaskCat.Lib.Utility.Odata;
using TaskCat.Model.Pagination;
using TaskCat.Lib.Utility;
using System.Web.Http.Description;
using TaskCat.Lib.Utility.ActionFilter;

namespace TaskCat.Controllers
{
    [Authorize(Roles = "Administrator, BackOfficeAdmin")]
    public class ProductCategoryController : ApiController
    {
        private IRepository<ProductCategory> service;

        public ProductCategoryController(IRepository<ProductCategory> service)
        {
            this.service = service;
        }

        [ResponseType(typeof(PageEnvelope<ProductCategory>))]
        [HttpGet]
        [Route("api/ProductCategory/odata", Name = AppConstants.ProductCategoryRoute)]
        [TaskCatOdataRoute]
        public async Task<IHttpActionResult> Get()
        {
            IQueryable<ProductCategory> productCategories = service.Collection.AsQueryable();

            var odataResult = await productCategories.ToOdataResponse(this.Request, AppConstants.ProductCategoryRoute);
            return Ok(odataResult);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Product Category Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await service.Get(id);
            return Ok(result);
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
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(
            [Required(AllowEmptyStrings = false, ErrorMessage = "Product Category Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await service.Delete(id);
            return Ok(result);
        }
    }
}
