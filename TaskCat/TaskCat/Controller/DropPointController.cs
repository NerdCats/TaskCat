namespace TaskCat.Controller
{
    using Lib.DropPoint;
    using System.Collections.Generic;
    using System.Web.Http;

    public class DropPointController : ApiController
    {
        private IDropPointService service;

        public DropPointController(IDropPointService service)
        {
            this.service = service;
        }
        // GET: api/DropPoint
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DropPoint/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DropPoint
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/DropPoint/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DropPoint/5
        public void Delete(int id)
        {
        }
    }
}
