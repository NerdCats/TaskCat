namespace TaskCat.Controller
{
    using Data.Entity;
    using Lib.DropPoint;
    using Microsoft.AspNet.Identity;
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
        public IHttpActionResult GetDropPointNameSuggestions()
        {
            return Json(DropPointNameSuggestions.Values);
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

            var authorizedId = User.Identity.GetUserId();
           
            
            if (value.UserId!=null && value.UserId != authorizedId
                && (!this.User.IsInRole("Administrator") || !this.User.IsInRole("BackOfficeAdmin")))
            {
                // TODO: Need to fix this differently by a proper result
                return Unauthorized();
            }
            value.Id = default(string);
            value.UserId = authorizedId;
            var result = await service.Insert(value);
            return Json(result);
        }

        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException("Method not implemented yet");
        }

        [HttpDelete]
        [Authorize]
        public async Task<DropPoint> Delete(string id)
        {
            var result = await service.Delete(id);
            return result;
        }
    }
}
