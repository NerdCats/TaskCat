namespace TaskCat.Account.Controllers
{
    using Core;
    using Data.Entity.Identity;
    using Data.Model;
    using Lib;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Client (any client that works with TaskCat) related controller
    /// </summary>
    public class ClientController : ApiController
    {
        private IClientStore store;

        /// <summary>
        /// Instantiates a instance of ClientController
        /// </summary>
        /// <param name="store"></param>
        public ClientController(IClientStore store)
        {
            this.store = store;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post(ClientModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Client newClient = await store.AddClient(model);
            return Ok<Client>(newClient);
        }
        
        [HttpPost]
        [Route("api/auth/Client/{id}/activate")]
        public async Task<IHttpActionResult> Activate([FromUri]string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"provided client id is null or empty");

            var result = await store.Activate(id);
            return Ok<Client>(result);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException($"provided client id is null or empty");

            var result = await store.DeleteClient(id);
            if (result)
                return StatusCode(HttpStatusCode.NoContent);
            else
                return StatusCode(HttpStatusCode.NotFound);
        }
    }
}
