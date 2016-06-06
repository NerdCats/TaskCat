namespace TaskCat.Controller
{
    using Data.Entity;
    using Lib.DropPoint;
    using Microsoft.AspNet.Identity;
    using Model.Response;
    using System;
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
        public string Get(string userId)
        {
            throw new NotImplementedException("Method not implemented yet");
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> Post([FromBody]DropPoint value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (value.Id != this.User.Identity.GetUserId() && (!this.User.IsInRole("Administrator") || !this.User.IsInRole("BackOfficeAdmin")))
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }

            throw new NotImplementedException("Method not implemented yet");
        }

        // PUT: api/DropPoint/5
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException("Method not implemented yet");
        }

        // DELETE: api/DropPoint/5
        public void Delete(int id)
        {
            throw new NotImplementedException("Method not implemented yet");
        }
    }
}
