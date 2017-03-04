using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using TaskCat.Data.Entity;
using TaskCat.Job;
using TaskCat.Warehouse.Lib;

namespace TaskCat.Warehouse.Controllers
{
    [RoutePrefix("api/warehouse")]
    public class InventoryController : ApiController
    {
        private IJobRepository jobRepo;
        private IStockService stockService;

        public InventoryController(IJobRepository jobRepo, IStockService stockService)
        {
            this.jobRepo = jobRepo;
            this.stockService = stockService;
        }

        /// <summary>
        /// Stock a new 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("stock")]
        public async Task<IHttpActionResult> Stock([FromBody]StockItem item)
        {
            var isReferenced = !string.IsNullOrWhiteSpace(item.RefId) && !string.IsNullOrWhiteSpace(item.RefEntityType);

            if(!isReferenced)
            {
                item.RefId = null;
                item.RefEntityType = null;
            }

           
            return Ok();
        }

        [HttpPost]
        [Route("register/{refEntity}/{refId}")]
        public async Task<IHttpActionResult> CheckIn([FromUri]string refEntity, [FromUri]string refId)
        {
            var job = await jobRepo.GetJobByIdOrHrid(refId);

            // INFO: We got the job now, the next thing we need to do 
            // is making sure whether we have this job referenced in the system

            var items = await this.stockService.GetStocksByReference(refId, refEntity);

            if (items == null || !items.Any())
            {
                // INFO: No items or anything here, send back them an option to check in.


            }
            else
            {

            }

            throw new NotImplementedException();
        }
    }
}
