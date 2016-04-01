namespace TaskCat.Controller
{
    using Data.Entity;
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Data.Model.Api;
    using TaskCat.Lib.Job;
    using Data.Model;
    using Lib.Constants;
    using Marvin.JsonPatch;
    using System.Web.OData.Query;
    using MongoDB.Driver;

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
        /// Returns A specific job request based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest();

                Job job = await _repository.GetJob(id);
                if (job == null) return NotFound();
                return Json(job);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
            try
            {
                var settings = new ODataValidationSettings()
                {
                    // Initialize settings as needed.
                    AllowedFunctions = AllowedFunctions.AllMathFunctions,
                    AllowedQueryOptions = AllowedQueryOptions.Count | AllowedQueryOptions.Filter | AllowedQueryOptions.OrderBy
                };

                query.Validate(settings);

                if (envelope)
                    return Json( await _repository.GetJobsEnveloped(query, page, pageSize, this.Request));
                return Json(await _repository.GetJobs(query, page, pageSize));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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

        [Route("api/Job/{jobId}/{taskId}")]
        [HttpPatch]
        public async Task<IHttpActionResult> Update([FromUri]string jobId, [FromUri] string taskId, [FromBody] JsonPatchDocument<JobTask> taskPatch)
        {
            try
            {
                ReplaceOneResult result = await _repository.UpdateJobWithPatch(jobId, taskId, taskPatch);
                return Json(result);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

    }
}
