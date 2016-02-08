namespace TaskCat.Controller
{
    using Data.Entity;
    using Lib.SupportedOrder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using TaskCat.Data.Model;
    using TaskCat.Lib.Order;

    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        private IOrderRepository _repository;
        private SupportedOrderRepository _suportedOrderRepository;

        public OrderController(IOrderRepository repository, SupportedOrderRepository supportedOrderRepository)
        {
            _repository = repository;
            _suportedOrderRepository = supportedOrderRepository;
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostOrder(OrderModel model)
        {
            if (model == null) return BadRequest("No freaking payload man!");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var orderResult = await _repository.PostOrder(model);
            return Json(orderResult);
        }

        [AllowAnonymous]
        [Route("SupportedOrder")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllSupportedOrder()
        {
            try
            {
                var supportedOrderList = await _suportedOrderRepository.GetAll();
                return Json(supportedOrderList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SupportedOrder")]
        [HttpPost]
        public async Task<IHttpActionResult> PostSupportedOrder(SupportedOrder supportedOrder)
        {
            try
            {
                await _suportedOrderRepository.PostSupportedOrder(supportedOrder);
                return Json(supportedOrder);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [AllowAnonymous]
        [Route("SupportedOrder")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSupportedOrderList(string id)
        {
            try
            {
                var supportedOrder = await _suportedOrderRepository.Get(id);
                return Json(supportedOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SupportedOrder")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateSupportedOrder(SupportedOrder order)
        {
            try
            {
                var updatedSupportedOrder = await _suportedOrderRepository.Update(order);
                return Json(updatedSupportedOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SupportedOrder")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteSupportedOrder(string id)
        {
            try
            {
                var deletedSupportedOrder = await _suportedOrderRepository.Delete(id);
                return Json(deletedSupportedOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
