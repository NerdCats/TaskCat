namespace TaskCat.Controllers
{
    using Common.Lib.Utility;
    using Common.Model.Pagination;
    using Common.Utility.ActionFilter;
    using Common.Utility.Odata;
    using Data.Entity;
    using Data.Model;
    using Job;
    using Lib.Comments;
    using Lib.Constants;
    using Microsoft.AspNet.Identity;
    using MongoDB.Driver;
    using NLog;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    /// <summary>
    /// Default controller to serve comments for any referenced entity
    /// </summary>
    public class CommentController : ApiController
    {
        private ICommentService service;
        private IJobRepository jobRepository;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a default instance of CommentController
        /// </summary>
        /// <param name="service">
        /// ICommentService to facilitate comment features
        /// </param>
        public CommentController(ICommentService service, IJobRepository jobRepository)
        {
            this.service = service;
            this.jobRepository = jobRepository;
        }

        /// <summary>
        /// Odata route to query comments
        /// </summary>
        /// <param name="pageSize">Page size to return results in. </param>
        /// <param name="page">Page number to return. </param>
        /// <param name="envelope">Boolean trigger to envelope or package the data in or not. </param>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Administrator,BackOfficeAdmin")]
        [ResponseType(typeof(PageEnvelope<Comment>))]
        [HttpGet]
        [Route("api/Comment/odata", Name = AppConstants.CommentOdataRoute)]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> Get()
        {
            IQueryable<Comment> comments = service.Collection.AsQueryable();

            var odataResult = await comments.ToOdataResponse(this.Request, AppConstants.CommentOdataRoute);
            return Ok(odataResult);
        }

        /// <summary>
        /// Get a single comment by id.
        /// </summary>
        /// <param name="id">Comment id to be fetched.</param>
        /// <returns>Comment with specified id.</returns>
        /// 
        [Authorize(Roles = "Administrator,BackOfficeAdmin")]
        [HttpGet]
        public async Task<IHttpActionResult> Get([Required(AllowEmptyStrings = false, ErrorMessage = "Comment Id not provided")]string id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var comment = await service.Get(id);   
            return Ok(comment);
        }

        /// <summary>
        /// Get comments by entity type and reference id which is ordered by create time. 
        /// </summary>
        /// <param name="entityType">Entity type the comment is associated with.</param>
        /// <param name="refId">Reference Id for the comment.</param>
        /// <param name="pageSize">Desired page size.</param>
        /// <param name="page">Desired page number.</param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpGet]
        [Route("api/Comment/{entityType}/{refId}", Name = AppConstants.DefaultCommentsRoute)]
        public async Task<IHttpActionResult> GetComments(string entityType, string refId, int pageSize = AppConstants.DefaultPageSize, int page = 0)
        {
            var currentUserId = this.User.Identity.GetUserId();
            if (service.IsValidEntityTypeForComment(entityType))
            {
                if (this.User.IsUserOrEnterpriseUserOnly() && entityType == typeof(Job).ToString())
                {
                    var job = await jobRepository.GetJobByHrid(refId);
                    if (job.User.UserId != currentUserId)
                    {
                        logger.Error("{0} is not allowed to get comment feed of {1}", currentUserId, refId);
                        throw new InvalidOperationException($"{currentUserId} is not allowed to get comment feed of {refId}");
                    }
                }
                var comments = await service.GetByRefId(refId, entityType, pageSize, page);
                return Ok(new PageEnvelope<Comment>(comments.Total, page, pageSize, AppConstants.DefaultCommentsRoute, comments.Result, this.Request));
            }

            return BadRequest("Wrong entity type provided");
        }

        /// <summary>
        /// Post request to create a comment.
        /// </summary>
        /// <param name="comment">Comment to be created. </param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> Post(Comment comment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = this.User.Identity.GetUserId();

            if (service.IsValidEntityTypeForComment(comment.EntityType))
            {
                // TODO: This needs to move to a proper class and place. Since I dont have any other entities that have 
                // comments enabled, this will do the trick for now
                if (this.User.IsUserOrEnterpriseUserOnly() && comment.EntityType == typeof(Job).ToString())
                {
                    var job = await jobRepository.GetJobByHrid(comment.RefId);
                    if (job.User.UserId != currentUserId)
                    {
                        logger.Error("{0} is not allowed to comment on {1}", currentUserId, comment.RefId);
                        throw new InvalidOperationException($"{currentUserId} is not allowed to comment on {comment.RefId}");
                    }
                }

                comment.User = new ReferenceUser(currentUserId, this.User.Identity.GetUserName())
                {
                    Name = this.User.Identity.GetUserFullName()
                };

                var result = await service.Insert(comment);
                return Created<Comment>($"{this.Request.RequestUri}{result.Id}", result);
            }
            return BadRequest("Wrong entity type provided");
        }

        /// <summary>
        /// Update a comment 
        /// </summary>
        /// <param name="model">CommentUpdateModel to update a single comment</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<IHttpActionResult> Update([FromBody] CommentUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = this.User.Identity.GetUserId();
            var comment = await service.Get(model.Id);

            if (!this.User.IsAdmin())
            {
                if (comment.User.Id != currentUserId)
                {
                    logger.Error("{0} is not allowed to update comment {1}", this.User.Identity.Name, model.Id);
                    throw new InvalidOperationException($"{this.User.Identity.Name} is not allowed to update comment {model.Id}");
                }
            }

            comment.LastModified = DateTime.UtcNow;
            comment.CommentText = model.CommentText;

            var result = await service.Update(comment);
            return Ok<Comment>(result);
        }

        /// <summary>
        /// Delete request to delete a comment.
        /// </summary>
        /// <param name="id">Delete to be created.</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            if (!this.User.IsAdmin())
            {
                var comment = await service.Get(id);
                if (comment.User.Id != currentUserId)
                {
                    logger.Error("{0} is not allowed to delete comment {1}", this.User.Identity.Name, id);
                    throw new InvalidOperationException($"{this.User.Identity.Name} is not allowed to delete comment {id}");
                }
            }

            var result = await service.Delete(id);
            return Ok<Comment>(result);
        }
    }
}