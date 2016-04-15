namespace TaskCat.Controller
{
    using Data.Entity;
    using Lib.Order;
    using Microsoft.AspNet.Identity;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Data.Model;
    using Data.Entity.Identity;
    using Data.Model.Order;

    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        private IOrderRepository _repository;

        public OrderController(IOrderRepository repository)
        {
            _repository = repository;
        }

        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> PostOrder(OrderModel model, OrderCreationOptions opt = OrderCreationOptions.CREATE)
        {
            try
            {
                if (model == null) return BadRequest("No freaking payload man!");
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var currentUserId = this.User.Identity.GetUserId();

                if (!this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR) 
                    && !this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN))
                {
                    if (model.UserId!=null && model.UserId != currentUserId)
                        throw new InvalidOperationException(string.Format("Updating user id {0} is not authorized against user id {1}", model.UserId, this.User.Identity.GetUserId()));
                    if (opt == OrderCreationOptions.CREATE_AND_CLAIM)
                        throw new InvalidOperationException(string.Format("Claiming a job under user id {0} is not authorized", User.Identity.GetUserId()));
                }

                if (model.UserId == null) model.UserId = currentUserId;

                Job createdJob;
                
                switch(opt)
                {
                    case OrderCreationOptions.CREATE:
                        createdJob = await _repository.PostOrder(model);
                        return Json(createdJob);
                    case OrderCreationOptions.CREATE_AND_CLAIM:
                        createdJob = await _repository.PostOrder(model, currentUserId);
                        return Json(createdJob);
                    default:
                        throw new InvalidOperationException("Invalid OrderCreationOptions selected");
                }
                
            }
            catch(ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AllowAnonymous]
        [Route("SupportedOrder")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllSupportedOrder()
        {
            try
            {
                var supportedOrderList = await _repository.GetAllSupportedOrder();
                return Json(supportedOrderList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("SupportedOrder")]
        [HttpPost]
        public async Task<IHttpActionResult> PostSupportedOrder(SupportedOrder supportedOrder)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _repository.PostSupportedOrder(supportedOrder);
                return Json(supportedOrder);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [AllowAnonymous]
        [Route("SupportedOrder/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSupportedOrder(string id)
        {
            try
            {
                var supportedOrder = await _repository.GetSupportedOrder(id);
                return Json(supportedOrder);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("SupportedOrder")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateSupportedOrder(SupportedOrder order)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedSupportedOrder = await _repository.UpdateSupportedOrder(order);
                return Json(updatedSupportedOrder);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("SupportedOrder/{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteSupportedOrder(string id)
        {
            try
            {
                var deletedSupportedOrder = await _repository.DeleteSupportedOrder(id);
                return Json(deletedSupportedOrder);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
