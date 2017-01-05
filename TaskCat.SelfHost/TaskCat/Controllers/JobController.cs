namespace TaskCat.Controllers
{
    using Common.Email;
    using Common.Lib.Utility;
    using Common.Model.Pagination;
    using Common.Utility.ActionFilter;
    using Common.Utility.Odata;
    using Data.Entity;
    using Data.Entity.Identity;
    using Data.Lib.Invoice.Request;
    using Data.Lib.Invoice.Response;
    using Data.Model;
    using Data.Model.Api;
    using Data.Model.Identity.Profile;
    using Data.Model.Job;
    using Data.Model.Operation;
    using Data.Model.Order;
    using Job;
    using Job.Invoice;
    using Job.Updaters;
    using Lib.Constants;
    using Marvin.JsonPatch;
    using Microsoft.AspNet.Identity;
    using MongoDB.Driver;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    /// <summary>
    /// Controller to Post Custom Jobs, List, Delete and Update Jobs 
    /// </summary>
    /// 
    public class JobController : ApiController
    {
        private IJobRepository repository;
        private IEmailService mailService;
        private Subject<JobActivity> activitySubject;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public JobController(IJobRepository repository, IEmailService mailService, Subject<JobActivity> activitySubject)
        {
            this.repository = repository;
            this.mailService = mailService;
            this.activitySubject = activitySubject;
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
            if (User != null && User.Identity.IsAuthenticated)
            {
                //FIXME: In this way an asset can see every job he is assigned or
                //not assigned to. Need to fix that

                Job job = null;
                if (id.Length == 24)
                    job = await repository.GetJob(id);
                else
                    job = await repository.GetJobByHrid(id);

                return Ok(job);
            }
            else
            {
                Job job = null;
                job = await repository.GetJobByHrid(id);
                return Ok(job);
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
                return BadRequest("Page size can't be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize;

            if (envelope)
                return Ok(await repository.GetJobsEnveloped(type, page, pageSize, this.Request, AppConstants.DefaultApiRoute));
            return Ok(await repository.GetJobs(type, page, pageSize));
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
        [Authorize]
        [ResponseType(typeof(PageEnvelope<Job>))]
        [Route("api/Job/odata", Name = AppConstants.JobsOdataRoute)]
        [HttpGet]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> ListOdata()
        {
            IQueryable<Job> jobs = repository.GetJobs();

            if (User.IsUserOrEnterpriseUserOnly())
            {
                // INFO: You're in this block because the user is either just a regular user 
                // or enterprise user , not an administrator or anything
                // so he is only entitled to get the jobs he ordered 

                jobs = jobs.Where(x => x.User.UserId == User.Identity.GetUserId()).AsQueryable();
            }

            var odataResult = await jobs.ToOdataResponse(Request, AppConstants.JobsOdataRoute);
            return Ok(odataResult);
        }

        /// <summary>
        /// Get Jobs assigned to an asset with odata query features
        /// </summary>
        /// <param name="assetId">Related asset id</param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, BackOfficeAdmin, Asset")]
        [Route("api/job/jobsbyasset/{assetId?}", Name = AppConstants.JobsByAssetOdataRoute)]
        [HttpGet]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> GetAssignedJobsByAssetId(string assetId = null)
        {
            var currentUserId = User.Identity.GetUserId();
            if (User.IsInRole(RoleNames.ROLE_ASSET) && !string.IsNullOrWhiteSpace(assetId) && assetId != currentUserId)
            {
                logger.Error("Asset {0} is not authorized to access job list assigned to {1}", User.Identity.Name, assetId);
                throw new UnauthorizedAccessException($"Asset {User.Identity.Name} is not authorized to access job list assigned to {assetId}");
            }

            assetId = string.IsNullOrEmpty(assetId) ? currentUserId : assetId;

            IQueryable<Job> jobs = repository.GetJobs().Where(x => x.Assets.ContainsKey(assetId)).AsQueryable();

            var odataResult = await jobs.ToOdataResponse(Request, AppConstants.JobsOdataRoute);
            return Ok(odataResult);
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
                {
                    logger.Debug("\'order.OrderCart\' is null");
                    logger.Error("Generating invoice with blank order cart is not supported");
                    throw new InvalidOperationException("Generating invoice with blank order cart is not supported");
                }

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
                logger.Error("Invoice for job type {0} is still not implemented", job.Order.Type);
                throw new NotImplementedException($"Invoice for job type {job.Order.Type} is still not implemented");
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
            return Ok(job);
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

            return Ok(result);
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
        [ResponseType(typeof(UpdateResult<Job>))]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("api/Job/claim/{jobId}")]
        [HttpPost]
        public async Task<IHttpActionResult> Claim(string jobId)
        {
            var job = await repository.GetJob(jobId);
            var result = await repository.Claim(job, this.User.Identity.GetUserId());

            Task.Run(() => this.activitySubject.OnNext(
                new JobActivity(job, JobActivityOperationNames.Claim, nameof(job.JobServedBy), new ReferenceUser(job.JobServedBy))));

            return Ok(result);
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
        [ResponseType(typeof(UpdateResult<Job>))]
        [Authorize(Roles = "Asset, Administrator, Enterprise, BackOfficeAdmin")]
        [Route("api/Job/{jobId}/{taskId}")]
        [HttpPatch]
        public async Task<IHttpActionResult> Update([FromUri]string jobId, [FromUri] string taskId, [FromBody] JsonPatchDocument<JobTask> taskPatch, [FromUri] bool updatedValue = false)
        {
            if (taskPatch == null)
            {
                logger.Error("\'taskPatch\' is null");
                throw new ArgumentNullException(nameof(taskPatch));
            }

            if (taskPatch.Operations.Any(x => x.OperationType != Marvin.JsonPatch.Operations.OperationType.Replace))
            {
                logger.Debug(taskPatch.Operations.ToString());
                logger.Error("Operations except replace is not supported");
                throw new NotSupportedException("Operations except replace is not supported");
            }

            // INFO: This is ghetto, need to do it in a better way, may be write extension methods for JsonPatchDocument
            List<string> allowedPaths = new List<string>();
            allowedPaths.Add(nameof(JobTask.AssetRef));
            allowedPaths.Add(nameof(JobTask.State));

            if (!taskPatch.Operations.All(x => allowedPaths.Any(a => x.path.EndsWith(a))))
            {
                logger.Error("Patch operation not supported on one or more paths");
                throw new NotSupportedException("Patch operation not supported on one or more paths");
            }

            var currentUser = new ReferenceUser(this.User.Identity.GetUserId(), this.User.Identity.GetUserName())
            {
                Name = this.User.Identity.GetUserFullName()
            };

            var job = await repository.GetJob(jobId);

            var activities = new List<JobActivity>();
            job.PropertyChanged += (sender, eventArgs) =>
            {
                JobActivity jobChangeActivity = null;
                switch (eventArgs.PropertyName)
                {
                    case nameof(Job.State):
                        jobChangeActivity = new JobActivity(job, JobActivityOperationNames.Update, nameof(Job.State), currentUser)
                        {
                            Value = (sender as Job).State.ToString()
                        };
                        activities.Add(jobChangeActivity);
                        break;
                }
            };

            var result = await repository.UpdateJobTaskWithPatch(job, taskId, taskPatch);

            result.SerializeUpdatedValue = updatedValue;

            var updatedTask = result.UpdatedValue.Tasks.First(x => x.id == taskId);

            var taskUpdates = new List<JobActivity>();
            foreach (var op in taskPatch.Operations)
            {
                var taskActivity = new JobActivity(
                    result.UpdatedValue,
                    JobActivityOperationNames.Update,
                    op.path.Substring(1),
                    currentUser,
                    new ReferenceActivity(taskId, updatedTask.Type))
                {
                    Value = op.value.ToString()
                };
                taskUpdates.Add(taskActivity);
            }

            activities.InsertRange(0, taskUpdates);

            Task.Factory.StartNew(() =>
            {
                foreach (var activity in activities)
                {
                    activitySubject.OnNext(activity);
                }
            });

            return Ok(result);
        }

        /// <summary>
        /// Cancel a job with specific job id
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(UpdateResult<Job>))]
        [HttpPost]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("api/Job/cancel")]
        public async Task<IHttpActionResult> CancelJob([FromBody]JobCancellationRequest request)
        {
            if (request == null) return BadRequest("null request encountered");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUser = new ReferenceUser(this.User.Identity.GetUserId(), this.User.Identity.GetUserName())
            {
                Name = this.User.Identity.GetUserFullName()
            };

            var job = await repository.GetJob(request.JobId);
            var result = await repository.CancelJob(job, request.Reason);

            Task.Factory.StartNew(() =>
            {
                var activity = new JobActivity(job, JobActivityOperationNames.Cancel, currentUser);
                activitySubject.OnNext(activity);
            });
            return Ok(result);
        }

        /// <summary>
        /// Restores a freezed job with a specific job id
        /// </summary>
        /// <remarks>
        /// A job can freeze itslef if its deleted or cancelled
        /// All outstanding and future changes for a job would be rejected unless it is restored
        /// </remarks>
        /// <returns></returns>
        /// 
        [ResponseType(typeof(UpdateResult<Job>))]
        [HttpPost]
        [Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [Route("api/Job/restore/{jobId}")]
        public async Task<IHttpActionResult> RestoreJob(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                logger.Error("Null or WhiteSpace in JobID: {0}", jobId);
                throw new ArgumentException(nameof(jobId));
            }

            var currentUser = new ReferenceUser(this.User.Identity.GetUserId(), this.User.Identity.GetUserName())
            {
                Name = this.User.Identity.GetUserFullName()
            };

            var job = await repository.GetJob(jobId);
            var result = await repository.RestoreJob(job);

            Task.Factory.StartNew(() =>
            {
                var activity = new JobActivity(job, JobActivityOperationNames.Restore, currentUser);
                activitySubject.OnNext(activity);
            });

            return Ok(result);
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
        [ResponseType(typeof(UpdateResult<Job>))]
        [Authorize]
        [Route("api/Job/{jobId}/order")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateOrder([FromUri]string jobId, [FromBody]OrderModel orderModel, [FromUri]string mode = JobUpdateMode.force)
        {
            if (!JobUpdateMode.IsValidUpdateMode(mode))
                throw new ArgumentException(nameof(mode));

            if (orderModel == null) return BadRequest("Null order payload provided");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = new ReferenceUser(currentUserId, this.User.Identity.GetUserName())
            {
                Name = this.User.Identity.GetUserFullName()
            };

            var job = await repository.GetJob(jobId);

            if (!this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR)
                && !this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN) && !this.User.IsInRole(RoleNames.ROLE_ASSET))
            {
                if (orderModel.UserId != null && orderModel.UserId != currentUserId)
                {
                    logger.Error("Invalid Operation: Updating user id {0} is not authorized against user id {1}",
                        orderModel.UserId, this.User.Identity.GetUserId());

                    throw new InvalidOperationException(string.Format(
                        "Updating user id {0} is not authorized against user id {1}",
                        orderModel.UserId, this.User.Identity.GetUserId()));
                }
            }
            else if (this.User.IsInRole(RoleNames.ROLE_ASSET)
                && !this.User.IsInRole(RoleNames.ROLE_ADMINISTRATOR)
                && !this.User.IsInRole(RoleNames.ROLE_BACKOFFICEADMIN))
            {
                if (!job.Assets.Any(x => x.Key == currentUserId))
                {
                    logger.Error("{0} is not an associated asset with this job", currentUserId);
                    throw new UnauthorizedAccessException($"{currentUserId} is not an associated asset with this job");
                }
            }

            var result = await repository.UpdateOrder(job, orderModel, mode);

            Task.Factory.StartNew(() =>
            {
                var activity = new JobActivity(job, JobActivityOperationNames.Update, nameof(Job.Order), currentUser);
                activitySubject.OnNext(activity);
            });

            return Ok(result);
        }
    }
}
