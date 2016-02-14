namespace TaskCat.Controller
{
    using Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Data.Model.Api;
    using TaskCat.Lib.Job;
    using Newtonsoft.Json.Linq;
    using Data.Model;
    using Lib.Constants;
    using Marvin.JsonPatch;
    using System.ComponentModel.DataAnnotations;

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
        public async Task<IHttpActionResult> List(string type="", int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = false)
        {
            if (pageSize == 0)
                return BadRequest("Page size cant be 0");
            if (page < 0)
                return BadRequest("Page index less than 0 provided");

            pageSize = pageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : pageSize = 25;

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


        [HttpPatch]
        public async Task<IHttpActionResult> Update([FromUri]string jobId, [FromUri] string taskId, [FromBody] JsonPatchDocument<JobTask> taskPatch)
        {
            var job = await _repository.GetJob(jobId);
            if (job == null) return BadRequest("Invalid Job Id provided");

            var selectedTask = job.Tasks.FirstOrDefault(x => x.id == taskId);
            if (selectedTask == null) return BadRequest("Invalid JobTask Id provided");

            taskPatch.ApplyTo(selectedTask);
            var result = await _repository.UpdateJobTask(job, selectedTask);
            return Json(result);
        }

    }
}
