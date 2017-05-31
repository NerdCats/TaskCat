namespace TaskCat.Controllers
{
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
    using Common.Model.Pagination;
    using Common.Utility.ActionFilter;
    using Common.Utility.Odata;
    using Data.Entity;
    using Job;
    using Lib.Constants;

    /// <summary>
    /// A basic controller to manage data tags
    /// </summary>
    public class DataTagController : ApiController
    {
        private readonly IDataTagService _service;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Instantiates a data tag service
        /// </summary>
        /// <param name="service">IDataTagService instance</param>
        public DataTagController(IDataTagService service)
        {
            this._service = service;
        }

        /// <summary>
        /// Odata route to search data tags
        /// </summary>
        /// <returns>paginated response based on the odata query</returns>
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

        /// <summary>
        /// Suggestion of data tag, to be used as an autocomplete source
        /// </summary>
        /// <param name="q">query string</param>
        /// <returns>data tags that matches the request</returns>
        [ResponseType(typeof(List<DataTag>))]
        [Route("api/DataTag/suggestions")]
        public async Task<IHttpActionResult> GetSuggestions(string q)
        {
            var suggestions = await _service.GetDataTagSuggestions(q);
            return Ok(suggestions);
        }

        /// <summary>
        /// Get a data tag or check for existence
        /// </summary>
        /// <param name="id">The data tag to be checked</param>
        /// <returns>The data tag if it exists</returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get([Required(AllowEmptyStrings = false, ErrorMessage = "Data tag id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var dataTag = await _service.Get(id);
            return Ok(dataTag);
        }

        /// <summary>
        /// Create a new data tag
        /// </summary>
        /// <param name="dataTag">DataTag object from the body.</param>
        /// <returns>The newly created DataTag</returns>
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]DataTag dataTag)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.Insert(dataTag);
            return Content(HttpStatusCode.Created, dataTag, new JsonMediaTypeFormatter());
        }


        /// <summary>
        /// Updates a data tag
        /// </summary>
        /// <param name="id">The old data tag</param>
        /// <param name="dataTag">the new data tag as a DataTag object in the body</param>
        /// <returns>The updated data tag</returns>
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [HttpPut]
        public async Task<IHttpActionResult> Put([Required(AllowEmptyStrings = false, ErrorMessage = "Data tag Id not provided")]string id, [FromBody]DataTag dataTag)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.Update(id, dataTag);
            return Ok(result);
        }

        /// <summary>
        /// Delete an existing data tag
        /// </summary>
        /// <param name="id">The data tag to be deleted</param>
        /// <returns>The deleted data tag</returns>
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
