using System.Web.Http;
using TaskCat.Job;

namespace TaskCat.Controllers
{
    public class TagController: ApiController
    {
        private readonly ITagService _service;

        public TagController(ITagService service)
        {
            this._service = service;
        }
    }
}
