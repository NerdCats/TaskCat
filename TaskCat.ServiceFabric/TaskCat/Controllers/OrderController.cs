using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using TaskCat.Data.Entity;
using TaskCat.Data.Entity.Identity;
using TaskCat.Data.Model;
using TaskCat.Data.Model.Order;
using TaskCat.Lib.Job;
using TaskCat.Lib.Order;

namespace TaskCat.Controllers
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        private IOrderRepository _repository;
        private IJobRepository _jobRepository;

        public OrderController(IOrderRepository repository, IJobRepository jobRepository)
        {
            _repository = repository;
            _jobRepository = jobRepository;
        }

        /// <summary>
        /// Default endpoint to create an Order
        /// </summary>
        /// <param name="model">
        /// OrderModel to be submitted, this can be anything that is inherited from OrderModel class
        /// </param>
        /// <param name="opt">
        /// Order create options, an admin can create an order and claim it himself too
        /// </param>
        /// <returns></returns>
        /// 
        [ResponseType(typeof(Job))]
        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> PostOrder(OrderModel model, OrderCreationOptions opt = OrderCreationOptions.CREATE)
        {
            if (model == null) return BadRequest("No freaking payload man!");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUserId = this.User.Identity.GetUserId();

            if (!this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR)
                && !this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN))
            {
                if (model.UserId != null && model.UserId != currentUserId)
                    throw new InvalidOperationException(string.Format("Updating order/job id {0} is not authorized against user id {1}", model.UserId, this.User.Identity.GetUserId()));
                if (opt == OrderCreationOptions.CREATE_AND_CLAIM)
                    throw new InvalidOperationException(string.Format("Claiming a job under user id {0} is not authorized", User.Identity.GetUserId()));
            }
                
            if (model.UserId == null) model.UserId = currentUserId;

            Job createdJob;

            switch (opt)
            {
                case OrderCreationOptions.CREATE:
                    createdJob = await _repository.PostOrder(model);
                    return Ok(createdJob);
                case OrderCreationOptions.CREATE_AND_CLAIM:
                    createdJob = await _repository.PostOrder(model, currentUserId);
                    return Ok(createdJob);
                default:
                    throw new InvalidOperationException("Invalid OrderCreationOptions selected");
            }
        }

        /// <summary>
        /// Gets List of the all supported order types
        /// </summary>
        /// <returns>
        /// List of supported orders
        /// </returns>
        /// 
        [ResponseType(typeof(SupportedOrder))]
        [AllowAnonymous]
        [Route("SupportedOrder")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllSupportedOrder()
        {
            var supportedOrderList = await _repository.GetAllSupportedOrder();
            return Ok(supportedOrderList);
        }

        /// <summary>
        /// Add a supported order in the system
        /// </summary>
        /// <param name="supportedOrder"></param>
        /// <returns>
        /// 
        /// </returns>
        /// 
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [ResponseType(typeof(SupportedOrder))]
        [Route("SupportedOrder")]
        [HttpPost]
        public async Task<IHttpActionResult> PostSupportedOrder(SupportedOrder supportedOrder)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _repository.PostSupportedOrder(supportedOrder);
            return Ok(supportedOrder);
        }

        /// <summary>
        /// Get a supported order by id
        /// </summary>
        /// <param name="id">
        /// id of the supported order entity
        /// </param>
        /// <returns>
        /// Supported Order of that respective id
        /// </returns>
        /// 
        [AllowAnonymous]
        [ResponseType(typeof(SupportedOrder))]
        [Route("SupportedOrder/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSupportedOrder(string id)
        {
             var supportedOrder = await _repository.GetSupportedOrder(id);
             return Ok(supportedOrder);         
        }

        /// <summary>
        /// Update a supported order
        /// </summary>
        /// <param name="order">
        /// SupportedOrder that needs to be updated
        /// </param>
        /// <returns>
        /// Updated Supported Order
        /// </returns>
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [ResponseType(typeof(SupportedOrder))]
        [Route("SupportedOrder")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateSupportedOrder(SupportedOrder order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedSupportedOrder = await _repository.UpdateSupportedOrder(order);
            return Ok(updatedSupportedOrder);
        }

        /// <summary>
        /// Delete a supported Order
        /// </summary>
        /// <param name="id">
        /// id of supported order to delete
        /// </param>
        /// <returns>
        /// Deleted Supported Order
        /// </returns>
        /// 
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [ResponseType(typeof(SupportedOrder))]
        [Route("SupportedOrder/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteSupportedOrder(string id)
        {
            var deletedSupportedOrder = await _repository.DeleteSupportedOrder(id);
            return Ok(deletedSupportedOrder);
        }

    }
}
