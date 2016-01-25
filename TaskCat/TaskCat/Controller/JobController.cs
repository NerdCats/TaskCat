﻿namespace TaskCat.Controller
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
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            JobEntity job = await _repository.GetJob(id);
            if (job == null) return NotFound();
            return Json(job);
        }
        

        [HttpGet]
        public async Task<IHttpActionResult> List(string type="", int start=0, int limit = 25)
        {
            if (limit > 25) limit = 25;
            try
            {
                return Json(await _repository.GetJobs(type, start, limit));
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
                JobEntity job = await _repository.PostJob(model);
                return Json(job);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPut]
        public async Task<IHttpActionResult> Update([FromUri]string id, [FromBody] JobTask task)
        {
            throw new NotImplementedException();
        }

    }
}
