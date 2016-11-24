namespace TaskCat.Controllers
{
    using MongoDB.Driver;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Common.Db;
    using Common.Model.Pagination;
    using Common.Utility.ActionFilter;
    using Data.Entity;
    using Lib.Constants;
    using Common.Utility.Odata;

    /// <summary>
    /// Controller for job activities.
    /// </summary>
    public class JobActivityController: ApiController
    {
        private IDbContext dbContext;

        /// <summary>
        /// Instantiates job activity controller.
        /// </summary>
        /// <param name="dbContext">Database context to go with.</param>
        public JobActivityController(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Default Odata route to get job activities
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "Administrator, BackOfficeAdmin")]
        [ResponseType(typeof(PageEnvelope<JobActivity>))]
        [HttpGet]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> GetJobActivityFeed()
        {
            IQueryable<JobActivity> activities = dbContext.JobActivityCollection.AsQueryable();
            var odataResult = await activities.ToOdataResponse(Request, AppConstants.JobActivityOdataRoute);
            return Ok(odataResult);
        }
    }
}
