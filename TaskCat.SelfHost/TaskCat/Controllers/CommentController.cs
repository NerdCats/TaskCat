namespace TaskCat.Controllers
{
    using Data.Entity;
    using Lib.Comments;
    using Lib.Constants;
    using Lib.Utility;
    using Lib.Utility.Odata;
    using LinqToQuerystring;
    using Model.Pagination;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Default controller to serve comments for any referenced entity
    /// </summary>
    public class CommentController : ApiController
    {
        private ICommentService service;

        /// <summary>
        /// Initializes a default instance of CommentController
        /// </summary>
        /// <param name="service">
        /// ICommentService to facilitate comment features
        /// </param>
        public CommentController(ICommentService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Odata route to query comments
        /// </summary>
        /// <param name="pageSize">Page size to return results in. </param>
        /// <param name="page">Page number to return. </param>
        /// <param name="envelope">Boolean trigger to envelope or package the data in or not. </param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Comment/odata", Name = AppConstants.CommentOdataRoute)]
        public IHttpActionResult Get(int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
        {
            PagingHelper.ValidatePageSize(AppConstants.MaxPageSize, pageSize, page);

            var odataQuery = this.Request.GetOdataQueryString(PagingQueryParameters.DefaultPagingParams);

            IQueryable<Comment> comments = service.Collection.AsQueryable();

            var queryTotal = comments.LinqToQuerystring(queryString: odataQuery);
            var queryResult = queryTotal.Skip(page * pageSize).Take(pageSize);

            if (envelope)
            {
                return Ok(new PageEnvelope<Comment>(queryTotal.LongCount(), page, pageSize, AppConstants.CommentOdataRoute, queryResult, this.Request));
            }
            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single comment by id.
        /// </summary>
        /// <param name="id">Comment id to be fetched.</param>
        /// <returns>Comment with specified id.</returns>
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
        [HttpGet]
        [Route("api/Comment/{entityType}/{refId}", Name = AppConstants.DefaultCommentsRoute)]
        public async Task<IHttpActionResult> GetComments(string entityType, string refId, int pageSize = AppConstants.DefaultPageSize, int page = 0)
        {
            if(service.IsValidEntityTypeForComment(entityType))
            {
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
        [HttpPost]
        public async Task<IHttpActionResult> Post(Comment comment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (service.IsValidEntityTypeForComment(comment.EntityType))
            {
                var result = await service.Insert(comment);
                return Created<Comment>($"{this.Request.RequestUri}{result.Id}", result);
            }
            return BadRequest("Wrong entity type provided");
        }

        /// <summary>
        /// Delete request to delete a comment.
        /// </summary>
        /// <param name="id">Delete to be created.</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IHttpActionResult> Delete (string id)
        {
            var result = await service.Delete(id);
            return Ok<Comment>(result);
        }
    }
}