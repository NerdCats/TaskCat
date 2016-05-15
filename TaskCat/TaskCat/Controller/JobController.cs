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
    using System.Collections.Generic;
    using Lib.Utility.Odata;
    using LinqToQuerystring;
    using Model.Pagination;
    using System.Web.Http.Description;
    using Lib.Email;

    /// <summary>
    /// Controller to Post Custom Jobs, List, Delete and Update Jobs 
    /// </summary>
    /// 
    public class JobController : ApiController
    {
        private IJobRepository repository;
        private IEmailService mailService;

        public JobController(IJobRepository repository, IEmailService mailService)
        {
            this.repository = repository;
            this.mailService = mailService;
        }

        /// <summary>
        /// Returns A specific job request based on id or human readable id
        /// This endpoint can be accessed being authorized or non-authorized
        /// If you are authorized as a Administrator or BackOfficeAdmin,
        /// You'd be able to search by id and hrid
        /// If you're accessing the non-authorized endpoint, you can only search 
        /// by hrid 
        /// </summary>
        /// <param name="id">
        /// job id to get
        /// </param>
        /// <returns>Job </returns>
        /// 
        [AllowAnonymous]
        [ResponseType(typeof(Job))]
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
                    job = await repository.GetJob(id);
                else
                    job = await repository.GetJobByHrid(id);

                return Json(job);
            }
            else
            {
                Job job = null;
                job = await repository.GetJobByHrid(id);
                return Json(job);
            }
        }

        /// <summary>
        /// List Jobs mostly with just type filter
        /// </summary>
        /// <param name="type">
        /// Job type to be filtered by
        /// </param>
        /// <param name="pageSize">
        /// Page Size to return results
        /// </param>
        /// <param name="page">
        /// page number to return results
        /// </param>
        /// <param name="envelope">
        /// envelope the job results in pagination header
        /// </param>
        /// <returns>
        /// Return Jobs matching type filter
        /// </returns>
        /// 
        [ResponseType(typeof(Job))]
        [Authorize(Roles = "Administrator, BackOfficeAdmin, Asset")]
        [HttpGet]
        public async Task<IHttpActionResult> List(string type = "", int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = false)
        {
            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize;

            if (envelope)
                return Json(await repository.GetJobsEnveloped(type, page, pageSize, this.Request));
            return Json(await repository.GetJobs(type, page, pageSize));
        }

        /// <summary>
        /// Odata powered query to get jobs
        /// </summary>
        /// <remarks>
        /// It would basically be a collection where all the odata queries are done with standard TaskCat Paging
        /// Supported Odata query are $count, $filter, $orderBy, $skip, $top  
        /// </remarks>
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
        ///
        [Authorize(Roles = "Asset, Administrator, BackOfficeAdmin")]
        [ResponseType(typeof(Job))]
        [Route("api/Job/odata", Name = AppConstants.DefaultOdataRoute)]
        [HttpGet]
        public async Task<IHttpActionResult> ListOdata(int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
        {
            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize;

            var queryParams = this.Request.GetQueryNameValuePairs();
            queryParams.VerifyQuery(new List<string>() {
                    OdataOptionExceptions.InlineCount,
                    OdataOptionExceptions.Skip,
                    OdataOptionExceptions.Top
                });

            var odataQuery = queryParams.GetOdataQuery(new List<string>() {
                    "pageSize",
                    "page",
                    "envelope"
                });

            var jobs = await repository.GetJobs();
            var queryResult = jobs.LinqToQuerystring(queryString: odataQuery).Skip(page * pageSize).Take(pageSize);

            if (envelope)
                return Json(new PageEnvelope<Job>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
            return Json(queryResult);
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
        /// 

        [Route("api/job/{jobhrid}/invoice")]
        [HttpGet]
        public async Task<IHttpActionResult> GenerateInvoiceForAJob(string jobhrid)
        {
            var job = await repository.GetJobByHrid(jobhrid);

            string customerName;
            if (job.User.Type == Data.Model.Identity.IdentityTypes.USER)
                customerName = (job.User.Profile as UserProfile)?.FullName;
            else if (job.User.Type == Data.Model.Identity.IdentityTypes.ENTERPRISE)
                customerName = (job.User.Profile as EnterpriseUserProfile)?.CompanyName;
            else
            {
                customerName = (job.User.Profile as AssetProfile)?.FullName;
            }

            if (job.Order.Type == OrderTypes.Delivery)
            {
                DeliveryOrder order = job.Order as DeliveryOrder;

                if (order.OrderCart == null)
                    throw new InvalidOperationException("Generating invoice with blank order cart is not supported");

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
                reponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
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
        /// <returns>
        /// </returns>
        [HttpPost]
        public async Task<IHttpActionResult> Post(JobModel model)
        {
            Job job = await repository.PostJob(model);
            return Json(job);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("api/job/notify/{jobId}")]
        public async Task<IHttpActionResult> Notify(string jobId)
        {
            Job job = await repository.GetJob(jobId);
            var result = await mailService.SendOrderMail(new SendEmailInvoiceRequest()
            {
                Job = job,
                RecipientEmail = job.User.Email,
                RecipientUsername = job.User.UserName
            });

            return Json(result);
        }

        /// <summary>
        /// Claim a job as a server 
        /// </summary>
        /// <remarks>
        /// This is used to claim a job 
        /// </remarks>
        /// <param name="jobId">
        /// Id for a job
        /// </param>
        /// <returns>
        /// Returns a replace result that replaces the JobServedBy field
        /// </returns>
        /// 
        [ResponseType(typeof(ReplaceOneResult))]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("api/Job/claim/{jobId}")]
        [HttpPost]
        public async Task<IHttpActionResult> Claim(string jobId)
        {
            ReplaceOneResult result = await repository.Claim(jobId, this.User.Identity.GetUserId());
            return Json(result);
        }

        /// <summary>
        /// Partial update to a specific task under a job
        /// </summary>
        /// <remarks>
        /// Patch update to a specific task to set a partial update 
        /// like changing assetRef or task state
        /// </remarks>
        /// <param name="jobId">
        /// Job Id the task is associated with
        /// </param>
        /// <param name="taskId"></param>
        /// <param name="taskPatch"></param>
        /// <returns>
        /// Returns a ReplaceOneResult based on the update
        /// </returns>
        /// 
        [ResponseType(typeof(ReplaceOneResult))]
        [Authorize(Roles = "Asset, Administrator, Enterprise, BackOfficeAdmin")]
        [Route("api/Job/{jobId}/{taskId}")]
        [HttpPatch]
        public async Task<IHttpActionResult> Update([FromUri]string jobId, [FromUri] string taskId, [FromBody] JsonPatchDocument<JobTask> taskPatch)
        {
            ReplaceOneResult result = await repository.UpdateJobWithPatch(jobId, taskId, taskPatch);
            return Json(result);
        }

        /// <summary>
        /// Update a order inside a job
        /// </summary>
        /// <param name="jobId">
        /// Job Id the task is associated with
        /// </param>
        /// <param name="orderModel">
        /// OrderModel derivative to update
        /// </param>
        /// <returns>
        /// Returns a ReplaceOneResult based on the update
        /// </returns>
        /// 
        [ResponseType(typeof(ReplaceOneResult))]
        [Authorize(Roles = "Asset, Administrator, Enterprise, BackOfficeAdmin")]
        [Route("api/Job/{jobId}/order")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrder([FromUri]string jobId, [FromBody]OrderModel orderModel)
        {
            ReplaceOneResult result = await repository.UpdateOrder(jobId, orderModel);
            return Json(result);
        }

    }
}
