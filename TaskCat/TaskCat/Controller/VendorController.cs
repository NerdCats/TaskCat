namespace TaskCat.Controller
{
    using Lib.Vendor;
    using Microsoft.AspNet.Identity;
    using System.Web.Http;
    using System;
    using Lib.Utility;
    using System.Threading.Tasks;
    using System.Net;
    using Data.Entity;

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
        public async Task<IHttpActionResult> Subscribe([FromBody]VendorProfile profile, [FromUri]string userId = null)
        {
            if (profile == null) throw new ArgumentNullException("Invalid/Null VendorProfile provided");

            var authorizedId = User.Identity.GetUserId();
            if (!User.IsAdmin() && !string.IsNullOrEmpty(userId) && authorizedId != userId)
                throw new InvalidOperationException($"User {authorizedId} is not authorized to subscribe for vendorship for user {userId}");

            var result = await service.Subscribe(userId, profile);

            switch (result)
            {
                case SubscriptionResult.SUCCESS:
                    return Ok();
                case SubscriptionResult.NOT_MODIFIED:
                    return StatusCode(HttpStatusCode.NotModified);
                case SubscriptionResult.FAILED:
                    throw new Exception("Subscribing as a vendor process failed");
                default:
                    throw new NotImplementedException($"Subscription result {result} is not supported/implemented");
            }
        }
    }
}
