namespace TaskCat.Controller
{
    using Data.Entity;
    using Lib.Constants;
    using Lib.DropPoint;
    using Lib.Utility.Odata;
    using LinqToQuerystring;
    using Microsoft.AspNet.Identity;
    using Model.Pagination;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class DropPointController : ApiController
    {
        private IDropPointService service;

        public DropPointController(IDropPointService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("api/DropPoint/suggestions")]
        public IHttpActionResult GetDropPointNameSuggestions()
        {
            return Json(DropPointNameSuggestions.Values);
        }

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

            IQueryable<DropPoint> dropPoints = service.Collection.AsQueryable();
            var queryResult = dropPoints.LinqToQuerystring(queryString: odataQuery)
                .Skip(page * pageSize)
                .Take(pageSize);

            if (envelope)
                return Json(new PageEnvelope<DropPoint>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Json(queryResult);
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> Post([FromBody]DropPoint value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorizedId = User.Identity.GetUserId();
           
            
            if (value.UserId!=null && value.UserId != authorizedId
                && (!this.User.IsInRole("Administrator") || !this.User.IsInRole("BackOfficeAdmin")))
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }
            value.Id = default(string);
            value.UserId = authorizedId;
            var result = await service.Insert(value);
            return Json(result);
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put(DropPoint value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await service.Update(value);
            return Json(result);
        }

        [HttpDelete]
        [Authorize]
        public async Task<DropPoint> Delete(string id)
        {
            var result = await service.Delete(id);
            return result;
        }
    }
}
