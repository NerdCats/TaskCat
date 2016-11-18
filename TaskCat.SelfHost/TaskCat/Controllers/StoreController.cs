using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using TaskCat.Data.Entity;
using TaskCat.Lib.Constants;
using TaskCat.Lib.Domain;
using TaskCat.Lib.Utility;
using System.Web.Http.Description;
using TaskCat.Common.Model.Pagination;
using TaskCat.Common.Utility.ActionFilter;
using TaskCat.Common.Utility.Odata;
using TaskCat.Common.Lib.Utility;

namespace TaskCat.Controllers
{
    public class StoreController : ApiController
    {
        private IRepository<Store> service;

        public StoreController(IRepository<Store> service)
        {
            this.service = service;
        }

        [ResponseType(typeof(PageEnvelope<Store>))]
        [HttpGet]
        [Route("api/Store/odata", Name = AppConstants.StoreOdataRoute)]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> Get()
        {
            IQueryable<Store> stores = service.Collection.AsQueryable();
            
            var odataResult = await stores.ToOdataResponse(this.Request, AppConstants.StoreOdataRoute);
            return Ok(odataResult);
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
