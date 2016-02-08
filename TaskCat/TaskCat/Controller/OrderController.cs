namespace TaskCat.Controller
{
    using Data.Entity;
    using Lib.Order;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using TaskCat.Data.Model;

    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        private IOrderRepository _repository;

        public OrderController(IOrderRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostOrder(OrderModel model)
        {
            try
            {
                if (model == null) return BadRequest("No freaking payload man!");
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var orderResult = await _repository.PostOrder(model);
                return Json(orderResult);
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
