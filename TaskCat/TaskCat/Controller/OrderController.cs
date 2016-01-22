namespace TaskCat.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using TaskCat.Data.Model;
    using TaskCat.Lib.Order;

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
            if (model == null) return BadRequest("No freaking payload man!");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var orderResult = await _repository.PostOrder(model);
            return Json(orderResult);
        }

    }
}
