﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LinqToQuerystring;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using TaskCat.Data.Entity;
using TaskCat.Lib.Constants;
using TaskCat.Lib.Utility;
using TaskCat.Lib.Utility.Odata;
using TaskCat.Lib.Vendor;
using TaskCat.Model.Pagination;
using TaskCat.Lib.Utility.ActionFilter;
using System.Web.Http.Description;

namespace TaskCat.Controllers
{
    [RoutePrefix("api/Vendor")]
    public class VendorController : ApiController
    {
        private IVendorService service;

        public VendorController(IVendorService service)
        {
            this.service = service;
        }

        [ResponseType(typeof(PageEnvelope<Vendor>))]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpGet]
        [TaskCatOdataRoute]
        public async Task<IHttpActionResult> Get(int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
        {
            IQueryable<Vendor> profiles = service.Collection.AsQueryable();
            var odataResult = await profiles.ToOdataResponse(this.Request, AppConstants.DefaultApiRoute);
            return Ok(odataResult);
        }

        [Authorize(Roles = "Administrator, Enterprise, BackOfficeAdmin")]
        [Route("Subscribe")]
        [HttpPost]
        public async Task<IHttpActionResult> Subscribe([FromBody]Vendor vendor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (vendor == null) throw new ArgumentNullException("Invalid/Null Vendor provided");

            var authorizedId = User.Identity.GetUserId();
            if (!User.IsAdmin() && !string.IsNullOrEmpty(vendor.UserId) && authorizedId != vendor.UserId)
                throw new InvalidOperationException($"User {authorizedId} is not authorized to subscribe for vendorship for user {vendor.UserId}");

            var result = await service.Subscribe(vendor);

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
            return Ok(result);
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var result = await service.Delete(id);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> Update([FromBody]Vendor vendor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await service.Update(vendor);
            return Ok(result);
        }
    }
}
