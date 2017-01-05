namespace TaskCat.Controllers
{
    using Job;
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
        public async Task<IHttpActionResult> GetLocalities(bool refresh = false)
        {
            if (refresh)
            {
                
            }

            return Ok();
        }
    }
}
