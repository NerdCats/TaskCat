namespace TaskCat.Controller
{
    using System.Web.Http;
    using Lib.Payment;
    using System.Web.Http.Description;
    using Data.Lib.Payment;

    /// <summary>
    /// <c>PaymentController</c> exposes all services provided by a <c>IPaymentManager</c>
    /// </summary>
    public class PaymentController : ApiController
    {
        private IPaymentManager manager;

        /// <summary>
        /// <c>PaymentController</c> constructor
        /// </summary>
        /// <param name="manager">
        /// <c>IPaymentManager instance to be injected in the controller</c>
        /// </param>
        public PaymentController(IPaymentManager manager)
        {
            this.manager = manager;
        }

        /// <summary>
        /// Get all the supported payment methods
        /// </summary>
        /// <returns>
        /// Returns all the payment method added in <c>PaymentManager</c>
        /// </returns>
        /// <remarks>
        /// Get all the supported payment methods
        /// </remarks>
        /// 

        [ResponseType(typeof(IPaymentMethod))]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Json(manager.GetPaymentMethods());
        }
    }
}
