namespace TaskCat.Controller
{
    using System.Web.Http;
    using Lib.Payment;
    using System.Web.Http.Description;
    using Data.Lib.Payment;
    using Lib.Job;
    using System;
    using System.Threading.Tasks;
    using Data.Lib.Payment.Request;
    using Lib.Exceptions;

    /// <summary>
    /// <c>PaymentController</c> exposes all services provided by a <c>IPaymentManager</c>
    /// </summary>
    public class PaymentController : ApiController
    {
        private IJobRepository jobRepository;
        private IPaymentManager manager;

        /// <summary>
        /// <c>PaymentController</c> constructor
        /// </summary>
        /// <param name="manager">
        /// <c>IPaymentManager instance to be injected in the controller</c>
        /// </param>
        public PaymentController(IPaymentManager manager, IJobRepository jobRepository)
        {
            this.manager = manager;
            this.jobRepository = jobRepository;
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
            return Json(manager.AllPaymentMethods);
        }

        /// <summary>
        /// Process Payment request of a job
        /// </summary>
        /// <param name="jobid">
        /// this is the job id that should get its payment processed
        /// </param>
        /// <returns></returns>
        /// 

        [Authorize(Roles = "Administrator, Asset, BackOfficeAdmin")]
        [HttpPost]
        [Route("api/payment/process/{jobid}")]
        public async Task<IHttpActionResult> Process(string jobid)
        {
            var job = await jobRepository.GetJob(jobid);
            var result = job.PaymentMethod.ProcessPayment(new ProcessPaymentRequest() {
                JobId = jobid
            });

            if (result.Errors != null && result.Errors.Count > 0)
                return Content(System.Net.HttpStatusCode.BadRequest, result.Errors);

            if (result.Success)
            {
                job.PaymentStatus = result.NewPaymentStatus;
                var jobUpdateResult = await jobRepository.UpdateJob(job);
                if (jobUpdateResult.ModifiedCount > 0)
                    return Ok();
                else
                    throw new ServerErrorException("job update failed");
            }
            else
            {
                return InternalServerError();
            }
        }
    }
}
