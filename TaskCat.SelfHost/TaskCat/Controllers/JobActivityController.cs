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
    using Common.Lib.Utility;
    using Data.Entity.Identity;
    using Microsoft.AspNet.Identity;

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
        [Authorize]
        [ResponseType(typeof(PageEnvelope<JobActivity>))]
        [HttpGet]
        [TaskCatOdataRoute(maxPageSize: AppConstants.MaxPageSize)]
        public async Task<IHttpActionResult> GetJobActivityFeed()
        {
            IQueryable<JobActivity> activities = null;
            if (!User.IsAdmin())
            {
                if (User.IsInRole(RoleNames.ROLE_ASSET))
                    activities = dbContext.JobActivityCollection.AsQueryable().Where(x => x.ByUser.Id == this.User.Identity.GetUserId());
                else
                    activities = dbContext.JobActivityCollection.AsQueryable().Where(x => x.ForUser.Id == this.User.Identity.GetUserId());
            }
            else
            {
                activities = dbContext.JobActivityCollection.AsQueryable();
            }
            
            var odataResult = await activities.ToOdataResponse(Request, AppConstants.DefaultApiRoute);
            return Ok(odataResult);
        }
    }
}
