namespace TaskCat.Controller
{
    using Lib.Vendor;
    using Microsoft.AspNet.Identity;
    using System.Web.Http;
    using System;
    using Lib.Utility;
    using System.Threading.Tasks;

    [RoutePrefix("api/Vendor")]
    public class VendorController : ApiController
    {
        private IVendorService service;

        public VendorController(IVendorService service)
        {
            this.service = service;
        }

        [Authorize(Roles = "Administrator, Enterprise, BackOfficeAdmin")]
        [Route("Subscribe")]
        [HttpPost]
        public async Task<IHttpActionResult> Subscribe(string userId = null)
        {
            var authorizedId = User.Identity.GetUserId();
            if (!User.IsAdmin() && !string.IsNullOrEmpty(userId) && authorizedId != userId)
                throw new InvalidOperationException($"User {authorizedId} is not authorized to subscribe for vendorship for user {userId}");

            bool result = await service.Subscribe(userId);

            if (result)
                return Ok();
            return StatusCode(System.Net.HttpStatusCode.Gone);          
        }
    }
}
