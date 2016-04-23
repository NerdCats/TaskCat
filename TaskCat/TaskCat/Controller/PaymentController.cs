namespace TaskCat.Controller
{
    using System.Web.Http;
    using Lib.Payment;

    public class PaymentController : ApiController
    {
        private IPaymentManager manager;

        public PaymentController(IPaymentManager manager)
        {
            this.manager = manager;
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            return Json(manager.GetPaymentMethods());
        }
    }
}
