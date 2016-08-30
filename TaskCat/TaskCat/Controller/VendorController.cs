﻿namespace TaskCat.Controller
{
    using Lib.Vendor;
    using Microsoft.AspNet.Identity;
    using System.Web.Http;
    using System;
    using Lib.Utility;
    using System.Threading.Tasks;
    using System.Net;
    using Data.Entity;
    using Lib.Constants;
    using System.Net.Http;
    using System.Collections.Generic;
    using Lib.Utility.Odata;
    using System.Linq;
    using MongoDB.Driver;
    using LinqToQuerystring;
    using Model.Pagination;

    [RoutePrefix("api/Vendor")]
    public class VendorController : ApiController
    {
        private IVendorService service;

        public VendorController(IVendorService service)
        {
            this.service = service;
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpGet]
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

            IQueryable<VendorProfile> profiles = service.Collection.AsQueryable();
            var queryResult = profiles.LinqToQuerystring(queryString: odataQuery)
                .Skip(page * pageSize)
                .Take(pageSize);

            if (envelope)
                return Json(new PageEnvelope<VendorProfile>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Json(queryResult);
        }

        [Authorize(Roles = "Administrator, Enterprise, BackOfficeAdmin")]
        [Route("Subscribe")]
        [HttpPost]
        public async Task<IHttpActionResult> Subscribe([FromBody]VendorProfile profile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (profile == null) throw new ArgumentNullException("Invalid/Null VendorProfile provided");

            var authorizedId = User.Identity.GetUserId();
            if (!User.IsAdmin() && !string.IsNullOrEmpty(profile.UserId) && authorizedId != profile.UserId)
                throw new InvalidOperationException($"User {authorizedId} is not authorized to subscribe for vendorship for user {profile.UserId}");

            var result = await service.Subscribe(profile);

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

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            var result = await service.Get(id);
            return Json(result);
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var result = await service.Delete(id);
            return Json(result);
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> Update([FromBody]VendorProfile profile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await service.Update(profile);
            return Json(result);
        }
    }
}
