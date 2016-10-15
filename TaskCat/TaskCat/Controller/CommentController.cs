namespace TaskCat.Controller
{
    using Data.Entity;
    using Lib.Comments;
    using Lib.Constants;
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

        [HttpGet]
        [Route("api/Comment/odata")]
        public IHttpActionResult Get(int pageSize = AppConstants.DefaultPageSize, int page = 0, bool envelope = true)
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

            IQueryable<Comment> comments = service.Collection.AsQueryable();
            var queryResult = comments.LinqToQuerystring(queryString: odataQuery)
                .Skip(page * pageSize)
                .Take(pageSize);

            if (envelope)
                return Ok(new PageEnvelope<Comment>(queryResult.LongCount(), page, pageSize, AppConstants.DefaultApiRoute, queryResult, this.Request));
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

    }
}
