namespace TaskCat.Controller
{
    using Microsoft.AspNet.Identity;
    using System.Collections.Generic;
    using System.Web.Http;
    using Data.Entity;
    using Lib.Store;
    using Lib.Utility;
    using System;
    using System.Threading.Tasks;
    using System.Net.Http.Formatting;
    using System.Net;
    using Lib.Constants;

    public class StoreController : ApiController
    {
        private IStoreService service;

        public StoreController(IStoreService service)
        {
            this.service = service;
        }

        // GET: api/Store
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            var stores = await service.Get(id);
            return Json(stores);
        }

        [Authorize(Roles = "Administrator, Enterprise, BackOcffficeAdmin")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Store store)
        {
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
        public async Task<IHttpActionResult> Delete(string id)
        {
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
