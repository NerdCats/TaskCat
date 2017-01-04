namespace TaskCat.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class LocalityController : ApiController
    {
        public LocalityController() { }

        [HttpGet]
        public async Task<IHttpActionResult> GetLocalities(bool refresh=false)
        {
            throw new NotImplementedException();
        }
    }
}
