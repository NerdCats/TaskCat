namespace TaskCat.Controller
{
    using Data.Entity;
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Data.Model.Api;
    using Lib.Job;
    using Data.Model;
    using Lib.Constants;
    using Marvin.JsonPatch;
    using System.Web.OData.Query;
    using MongoDB.Driver;
    using Microsoft.AspNet.Identity;
    using Data.Model.Order;
    using Lib.Invoice;
    using Lib.Invoice.Request;
    using Data.Lib.Invoice.Response;
    using Data.Model.Identity.Profile;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    /// <summary>
    /// Controller to Post Custom Jobs, List, Delete and Update Jobs 
    /// </summary>
    /// 
    public class JobController : ApiController
    {
        private IJobRepository _repository;

        public JobController(IJobRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Returns A specific job request based on id or human readable id
        /// This endpoint can be accessed being authorized or non-authorized
        /// If you are authorized as a Administrator or BackOfficeAdmin,
        /// You'd be able to search by id and hrid
        /// If you're accessing the non-authorized endpoint, you can only search 
        /// by hrid 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [Authorize(Roles = "Administrator, BackOfficeAdmin, Asset")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();
            if (User.Identity.IsAuthenticated)
            {
                //FIXME: In this way an asset can see every job he is assigned or
                //not assigned to. Need to fix that

                Job job = null;
                if (id.Length == 24)
                    job = await _repository.GetJob(id);
                else
                    job = await _repository.GetJobByHrid(id);

                return Json(job);
            }
            else
            {
                Job job = null;
                job = await _repository.GetJobByHrid(id);
                return Json(job);
            }
        }


        [HttpGet]
        public async Task<IHttpActionResult> List(string type = "", int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = false)
        {
            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize;

            try
            {
                if (envelope)
                    return Json(await _repository.GetJobsEnveloped(type, page, pageSize, this.Request));
                return Json(await _repository.GetJobs(type, page, pageSize));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Odata powered query to get jobs
        /// </summary>
        /// <param name="query">
        /// It would basically be a collection where all the odata queries are done with standard TaskCat Paging
        /// Supported Odata query are $count, $filter, $orderBy, $skip, $top  
        /// </param>
        /// <param name="pageSize">
        /// pageSize for a single page
        /// </param>
        /// <param name="page">
        /// page number to be fetched
        /// </param>
        /// <param name="envelope">
        /// By default this is true, given false, the result comes as not paged
        /// </param>
        /// <returns>
        /// A list of Jobs that complies with the query
        /// </returns>

        [Route("api/Job/odata", Name = AppConstants.DefaultOdataRoute)]
        [HttpGet]
        public async Task<IHttpActionResult> ListOdata(ODataQueryOptions<Job> query, int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
        {

            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize;

            var settings = new ODataValidationSettings()
            {
                // Initialize settings as needed.
                AllowedFunctions = AllowedFunctions.AllMathFunctions,
                AllowedQueryOptions = AllowedQueryOptions.Count | AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy
            };

            query.Validate(settings);

            if (envelope)
                return Json(await _repository.GetJobsEnveloped(query, page, pageSize, this.Request));
            return Json(await _repository.GetJobs(query, page, pageSize));
        }

        /// <summary>
        /// Generate an invoice against the job hrid given in here
        /// </summary>
        /// <param name="jobhrid">
        /// Human Readable ID for a job
        /// </param>
        /// <returns>
        /// A invoice for a job 
        /// </returns>
        [Route("api/job/{jobhrid}/invoice")]
        [HttpGet]
        public async Task<IHttpActionResult> GenerateInvoiceForAJob(string jobhrid)
        {
            var job = await _repository.GetJobByHrid(jobhrid);

            string customerName;
            if (job.User.Type == Data.Model.Identity.IdentityTypes.USER)
                customerName = (job.User.Profile as UserProfile).FullName;
            else if (job.User.Type == Data.Model.Identity.IdentityTypes.ENTERPRISE)
                customerName = (job.User.Profile as EnterpriseUserProfile).CompanyName;
            else
            {
                customerName = (job.User.Profile as AssetProfile).FullName;
            }

            if (job.Order.Type == OrderTypes.Delivery)
            {
                DeliveryOrder order = job.Order as DeliveryOrder;
                IInvoiceService invoiceService = new InvoiceService();
                DeliveryInvoice invoice = invoiceService.GenerateInvoice<ItemDetailsInvoiceRequest, DeliveryInvoice>(new ItemDetailsInvoiceRequest()
                {
                    CustomerName = customerName,
                    DeliveryFrom = order.From,
                    DeliveryTo = order.To,
                    ItemDetails = order.OrderCart.PackageList,
                    NetTotal = order.OrderCart.PackageList.Sum(x => x.Total),
                    NotesToDeliveryMan = order.NoteToDeliveryMan,
                    PaymentStatus = job.PaymentStatus,
                    ServiceCharge = order.OrderCart.ServiceCharge,
                    SubTotal = order.OrderCart.SubTotal,
                    TotalToPay = order.OrderCart.TotalToPay,
                    TotalVATAmount = order.OrderCart.TotalVATAmount,
                    TotalWeight = order.OrderCart.TotalWeight,
                    VendorName = "Anonymous"
                });

                invoice.InvoiceId = job.HRID;
                IPDFService<DeliveryInvoice> DeliveryInvoicePrinter = new DeliveryInvoicePDFGenerator();
                var invoiceStream = DeliveryInvoicePrinter.GeneratePDF(invoice);

                var reponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(invoiceStream.GetBuffer())
                };
                reponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Concat(invoice.InvoiceId, ".pdf")
                };
                reponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return ResponseMessage(reponseMessage);
            }
            else
            {
                throw new NotImplementedException("Invoice for this job type is still not implemented");
            }
        }


        /// <summary>
        /// Post a generic job payload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Post(JobModel model)
        {
            try
            {
                Job job = await _repository.PostJob(model);
                return Json(job);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("api/Job/claim/{jobId}")]
        [HttpPost]
        public async Task<IHttpActionResult> Claim(string jobId)
        {
            ReplaceOneResult result = await _repository.Claim(jobId, this.User.Identity.GetUserId());
            return Json(result);
        }

        [Authorize(Roles = "Asset, Administrator, Enterprise, BackOfficeAdmin")]
        [Route("api/Job/{jobId}/{taskId}")]
        [HttpPatch]
        public async Task<IHttpActionResult> Update([FromUri]string jobId, [FromUri] string taskId, [FromBody] JsonPatchDocument<JobTask> taskPatch)
        {
            try
            {
                ReplaceOneResult result = await _repository.UpdateJobWithPatch(jobId, taskId, taskPatch);
                return Json(result);
            }
            catch (InvalidOperationException ex)
            {
                return Content(System.Net.HttpStatusCode.Forbidden, ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
