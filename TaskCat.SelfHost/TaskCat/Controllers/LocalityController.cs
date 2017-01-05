namespace TaskCat.Controllers
{
    using Job;
    using Common.Model.Pagination;
    using Common.Utility.ActionFilter;
    using Common.Utility.Odata;
    using Data.Entity;
    using Lib.Constants;
    using MongoDB.Driver;
    using System.Web.Http.Description;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Localities fetched from jobs
    /// </summary>
    public class LocalityController : ApiController
    {
        private readonly ILocalityService _service;

        /// <summary>
        /// Creates a single instance of LocalityController.
        /// It's a controller for handling localities used in jobs.
        /// </summary>
        /// <param name="service"></param>
        public LocalityController(ILocalityService service)
        {
            this._service = service;
        }

        /// <summary>
        /// Get the current set of localities used in jobs. 
        /// </summary>
        /// <param name="refresh">Set to true for refreshing the list of localities. Default is false.</param>
        /// <returns>A list of </returns>
        [Authorize]
        [ResponseType(typeof(PageEnvelope<Locality>))]
        [HttpGet]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> GetLocalities(bool refresh = false)
        {
            if (refresh)
            {
                await _service.RefreshLocalities();
            }

            var localities = _service.Collection.AsQueryable();
            var result = await localities.ToOdataResponse(Request, AppConstants.DefaultApiRoute);
            return Ok(result);
        }
    }
}
