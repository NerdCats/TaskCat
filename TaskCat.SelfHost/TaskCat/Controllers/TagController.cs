using MongoDB.Driver;
using NLog;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TaskCat.Common.Model.Pagination;
using TaskCat.Common.Utility.ActionFilter;
using TaskCat.Common.Utility.Odata;
using TaskCat.Data.Entity;
using TaskCat.Job;
using TaskCat.Lib.Constants;

namespace TaskCat.Controllers
{
    public class TagController : ApiController
    {
        private readonly IDataTagService _service;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public TagController(IDataTagService service)
        {
            this._service = service;
        }

        [ResponseType(typeof(PageEnvelope<DataTag>))]
        [HttpGet]
        [Route("api/DataTag/odata", Name = AppConstants.DataTagOdataRoute)]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> Get()
        {
            IQueryable<DataTag> stores = _service.Collection.AsQueryable();

            var odataResult = await stores.ToOdataResponse(this.Request, AppConstants.DataTagOdataRoute);
            return Ok(odataResult);
        }

        [ResponseType(typeof(List<DataTag>))]
        [Route("api/DataTag/suggestions")]
        public async Task<IHttpActionResult> GetSuggestions(string q)
        {
            var suggestions = await _service.GetDataTagSuggestions(q);
            return Ok(suggestions);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([Required(AllowEmptyStrings = false, ErrorMessage = "Data tag id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var dataTag = await _service.Get(id);
            return Ok(dataTag);
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]DataTag dataTag)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.Insert(dataTag);
            return Content(HttpStatusCode.Created, dataTag, new JsonMediaTypeFormatter());
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody]DataTag dataTag)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.Update(dataTag);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator, BackOfficAdmin")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([Required(AllowEmptyStrings = false, ErrorMessage = "Data tag Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.Delete(id);
            return Content(HttpStatusCode.Accepted, result, new JsonMediaTypeFormatter());
        }
    }
}
