namespace TaskCat.Controllers
{
    using Microsoft.AspNet.Identity;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Common.Exceptions;
    using Common.Lib.Utility;
    using Data.Entity;
    using Data.Lib.Payment;
    using Data.Lib.Payment.Request;
    using Payment.Core;
    using Job;
    using NLog;

    /// <summary>
    /// <c>PaymentController</c> exposes all services provided by a <c>IPaymentManager</c>
    /// </summary>
    public class PaymentController : ApiController
    {
        private Subject<JobActivity> activitySubject;
        private IJobRepository jobRepository;
        private IPaymentManager manager;
        private IPaymentService service;
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// <c>PaymentController</c> constructor
        /// </summary>
        /// <param name="manager">
        /// <c>IPaymentManager instance to be injected in the controller</c>
        /// </param>
        public PaymentController(IPaymentManager manager, IPaymentService service, IJobRepository jobRepository, Subject<JobActivity> activitySubject)
        {
            this.service = service;
            this.manager = manager;
            this.jobRepository = jobRepository;
            this.activitySubject = activitySubject;
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
            return Ok(manager.AllPaymentMethods);
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
            var paymentMethod = service.GetPaymentMethodByKey(job.PaymentMethod);
            var result = paymentMethod.ProcessPayment(new ProcessPaymentRequest()
            {
                JobId = jobid
            });

            var currentUser = new ReferenceUser(this.User.Identity.GetUserId(), this.User.Identity.GetUserName())
            {
                Name = this.User.Identity.GetUserFullName()
            };

            if (result.Errors != null && result.Errors.Count > 0)
                return Content(System.Net.HttpStatusCode.BadRequest, result.Errors);

            if (result.Success)
            {
                job.PaymentStatus = result.NewPaymentStatus;
                var jobUpdateResult = await jobRepository.UpdateJob(job);

                Task.Factory.StartNew(() =>
                {
                    var activity = new JobActivity(job, JobActivityOperationNames.Update, nameof(Job.PaymentStatus), currentUser)
                    {
                        Value = result.NewPaymentStatus.ToString()
                    };
                    activitySubject.OnNext(activity);
                });

                if (jobUpdateResult.ModifiedCount > 0)
                    return Ok();
                else
                {
                    logger.Error("Job update failed for JobId: {0}", jobid);
                    throw new ServerErrorException("job update failed");
                }
            }
            else
            {
                logger.Error("Internal Server Error");
                return InternalServerError();
            }
        }
    }
}
