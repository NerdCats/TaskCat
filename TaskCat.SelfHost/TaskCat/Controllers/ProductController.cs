namespace TaskCat.Controllers
{
    using Common.Domain;
    using Common.Lib.Utility;
    using Common.Model.Pagination;
    using Common.Utility.ActionFilter;
    using Common.Utility.Odata;
    using Data.Entity;
    using Lib.Constants;
    using Microsoft.AspNet.Identity;
    using MongoDB.Driver;
    using NLog;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    public class ProductController : ApiController
    {
        private IRepository<Product> productService;
        private IRepository<DataTag> storeService;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ProductController(IRepository<Product> productService, IRepository<DataTag> storeService)
        {
            this.productService = productService;
            this.storeService = storeService;
        }

        [HttpGet]
        [Route("api/Product/odata", Name = AppConstants.ProductOdataRoute)]
        [ResponseType(typeof(PageEnvelope<Product>))]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> Get()
        {
            IQueryable<Product> products = productService.Collection.AsQueryable();
            var odataResult = await products.ToOdataResponse(this.Request, AppConstants.ProductOdataRoute);
            return Ok(odataResult);
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

            if (!User.IsAdminOrBackOfficeAdmin())
            {
                if (authorizedId != store.EnterpriseUserId)
                {
                    logger.Error("INVALID OPERATION: User {0} is not authorized to enlist a product in {1}",
                        authorizedId, store.Name);

                    throw new InvalidOperationException($"User {authorizedId} is not authorized to enlist a product in {store.Name}");
                }
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

            if (!User.IsAdminOrBackOfficeAdmin())
            {
                if (authorizedId != store.EnterpriseUserId)
                {
                    logger.Error("INVALID OPERATION: User {0} is not authorized to update a product in {1}",
                        authorizedId, store.Name);

                    throw new InvalidOperationException($"User {authorizedId} is not authorized to update a product in {store.Name}");
                }
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

            if (!User.IsAdminOrBackOfficeAdmin())
            {
                if (authorizedId != store.EnterpriseUserId)
                {
                    logger.Error("User {0} is not authorized to delete a product in {1}",
                        authorizedId, store.Name);

                    throw new InvalidOperationException($"User {authorizedId} is not authorized to delete a product in {store.Name}");
                }
            }

            var result = await productService.Delete(id);
            return Ok(result);
        }
    }
}
