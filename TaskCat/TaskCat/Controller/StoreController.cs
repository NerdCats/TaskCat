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

        // GET: api/Store/5
        public string Get(int id)
        {
            return "value";
        }

        [Authorize(Roles = "Administrator, Enterprise, BackOfficeAdmin")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Store store)
        {
            var authorizedId = this.User.Identity.GetUserId();
            if (!User.IsAdmin())
            {
                if (authorizedId != store.EnterpriseUserId)
                    throw new InvalidOperationException($"User {authorizedId} is not authorized to subscribe for vendorship for user {store.EnterpriseUserId}");

                store.DisplayOrder = AppConstants.DefaultStoreOrder;
            }

            var result = await service.Insert(store);
            return Content<Store>(HttpStatusCode.Created, store, new JsonMediaTypeFormatter());
        }

        // PUT: api/Store/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/Store/5
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
