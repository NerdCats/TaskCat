using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TaskCat.Data.Entity;
using TaskCat.Lib.SupportedOrder;

namespace TaskCat.Controller
{
    public class SupportedOrderController : ApiController
    {
        private SupportedOrderRepository _repository;
        public SupportedOrderController(SupportedOrderRepository repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var supportedOrderList = await _repository.GetAll();
                return Json(supportedOrderList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post(SupportedOrder supportedOrder)
        {
            try
            {
                await _repository.PostSupportedOrder(supportedOrder);
                return Json(supportedOrder);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }
    }
}
