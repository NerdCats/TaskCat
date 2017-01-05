namespace TaskCat.Controllers
{
    using Job;
    using Common.Utility.ActionFilter;
    using Common.Utility.Odata;
    using Lib.Constants;
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class LocalityController : ApiController
    {
        private ILocalityService service;

        public LocalityController(ILocalityService service)
        {
            this.service = service;
        }

        [HttpGet]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> GetLocalities(bool refresh = false)
        {
            if (refresh)
            {
                await service.RefreshLocalities();
                return Ok();
            }

            var localities = service.Collection.AsQueryable();
            var result = await localities.ToOdataResponse(Request, AppConstants.DefaultApiRoute);
            return Ok(result);
        }
    }
}
