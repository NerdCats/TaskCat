using System.Web.Http;
using TaskCat.Job;

namespace TaskCat.Controllers
{
    public class TagController: ApiController
    {
        private readonly IDataTagService _service;

        public TagController(IDataTagService service)
        {
            this._service = service;
        }
    }
}
