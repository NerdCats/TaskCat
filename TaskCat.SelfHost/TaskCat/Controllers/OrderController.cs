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
using System.Reactive.Subjects;

namespace TaskCat.Controllers
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        private IOrderRepository repository;
        private IJobRepository jobRepository;
        private Subject<JobActivity> activitySubject;

        public OrderController(
            IOrderRepository repository, 
            IJobRepository jobRepository, 
            Subject<JobActivity> activitySubject)
        {
            this.repository = repository;
            this.jobRepository = jobRepository;
            this.activitySubject = activitySubject;
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
                    throw new InvalidOperationException($"Updating order/job id {model.UserId} is not authorized against user id {this.User.Identity.GetUserId()}");
                if (opt == OrderCreationOptions.CREATE_AND_CLAIM)
                    throw new InvalidOperationException($"Claiming a job under user id {User.Identity.GetUserId()} is not authorized");
            }
                
            if (model.UserId == null) model.UserId = currentUserId;

            Job createdJob;
            var referenceUserForActivityLog = new ReferenceUser(currentUserId, this.User.Identity.GetUserName());

            switch (opt)
            {
                case OrderCreationOptions.CREATE:
                    createdJob = await repository.PostOrder(model);
                    activitySubject.OnNext(new JobActivity(createdJob, JobActivityOperatioNames.Create, referenceUserForActivityLog));
                    return Ok(createdJob);
                case OrderCreationOptions.CREATE_AND_CLAIM:
                    createdJob = await repository.PostOrder(model, currentUserId);
                    activitySubject.OnNext(new JobActivity(createdJob, JobActivityOperatioNames.Create, referenceUserForActivityLog));
                    activitySubject.OnNext(new JobActivity(createdJob, JobActivityOperatioNames.Claim, referenceUserForActivityLog));
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
            var supportedOrderList = await repository.GetAllSupportedOrder();
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

            await repository.PostSupportedOrder(supportedOrder);
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
             var supportedOrder = await repository.GetSupportedOrder(id);
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

            var updatedSupportedOrder = await repository.UpdateSupportedOrder(order);
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
            var deletedSupportedOrder = await repository.DeleteSupportedOrder(id);
            return Ok(deletedSupportedOrder);
        }

    }
}
