namespace TaskCat.Warehouse.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Data.Entity;
    using Data.Model;
    using Job;
    using Lib;

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
        /// Stock a new stock item. If Reference id is used, use a reference type too. Otherwise it would be ignored
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("stock")]
        public async Task<IHttpActionResult> Stock([FromBody]StockItemModel itemModel)
        {
            var result = await stockService.Insert(StockItem.FromModel(itemModel));
            return Ok(result);
        }


        [HttpPost]
        [Route("register/{refEntity}/{refId}")]
        public async Task<IHttpActionResult> CheckIn([FromUri]string refEntity, [FromUri]string refId)
        {
            var job = await jobRepo.GetJobByIdOrHrid(refId);

            // INFO: We got the job now, the next thing we need to do 
            // is making sure whether we have this job referenced in the system

            var items = await this.stockService.GetStocksByReference(refId, refEntity);
            // INFO: No items or anything here, send back them an option to check in.

            if (items == null || !items.Any())
            {
                var result = items.Select(item => new WarehouseOperation()
                {
                    Op = "stock",
                    Payload = new StockItemModel()
                    {
                        Item = item.Item,
                        PicUrl = item.PicUrl,
                        Quantity = item.Quantity,
                        RefEntityType = item.RefEntityType,
                        RefId = item.RefId
                    }
                });

                return Ok(result);      
            }
            else
            {
                // Find out which one of the products are not checked in yet and send back operations to do so
                var itemsInStock = items.GroupBy(x => x.Item).Select(x => x.Key);
                var itemsInJob = items.GroupBy(x => x.Item).Select(x => x.Key);

                foreach (var item in itemsInStock)
                {

                }

                var newThingsInJob = itemsInStock.Union(itemsInJob).Except(itemsInStock);
            }
        }
    }
}
