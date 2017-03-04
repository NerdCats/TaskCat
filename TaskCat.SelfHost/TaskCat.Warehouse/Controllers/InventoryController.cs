namespace TaskCat.Warehouse.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Data.Entity;
    using Data.Model;
    using Job;
    using Lib;
    using System.Collections.Generic;

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

        [HttpPut]
        [Route("stock")]
        public async Task<IHttpActionResult> UpdateStock([FromBody]StockItem item)
        {
            var result = await stockService.Update(item);
            return Ok(result);
        }

        [HttpPost]
        [Route("checkin/{refId}")]
        public async Task<IHttpActionResult> CheckIn([FromUri]string refId)
        {
            var job = await jobRepo.GetJobByIdOrHrid(refId);

            // INFO: We got the job now, the next thing we need to do 
            // is making sure whether we have this job referenced in the system

            // The reason we list out Entity type is in future we might need a different
            // entity to have stocks in the system. For now, this is hard coded, when
            // there comes a scenario where we have multiple entities, we can reuse this.
            var items = await this.stockService.GetStocksByReference(refId, "Job");

            // INFO: No items or anything here, send back them an option to check in.
            if (items == null || !items.Any())
            {
                var result = job.Order.OrderCart.PackageList.Select(item => new WarehouseOperation()
                {
                    Op = "insert",
                    Payload = new StockItemModel()
                    {
                        Item = item.Item,
                        PicUrl = item.PicUrl,
                        Quantity = item.Quantity,
                        RefEntityType = "Job",
                        RefId = refId
                    }
                });

                return Ok(result);
            }
            else
            {
                var SuggestedOperations = new List<WarehouseOperation>();

                var itemDict = items.ToDictionary(x => x.Item, x => x.Quantity);
                foreach (var item in job.Order.OrderCart.PackageList)
                {
                    if (!itemDict.ContainsKey(item.Item))
                    {
                        SuggestedOperations.Add(new WarehouseOperation()
                        {
                            Op = "insert",
                            Payload = new StockItemModel()
                            {
                                Item = item.Item,
                                PicUrl = item.PicUrl,
                                Quantity = item.Quantity,
                                RefEntityType = "Job",
                                RefId = refId
                            }
                        });
                    }
                    else
                    {
                        var itemQtyDiff = item.Quantity - itemDict[item.Item];
                        if (itemQtyDiff > 0)
                        {
                            for (int i = 0; i < itemQtyDiff; i++)
                            {
                                SuggestedOperations.Add(new WarehouseOperation()
                                {
                                    Op = "update",
                                    Payload = new StockItemModel()
                                    {
                                        Item = item.Item,
                                        PicUrl = item.PicUrl,
                                        Quantity = item.Quantity,
                                        RefEntityType = "Job",
                                        RefId = refId
                                    }
                                });
                            }
                        }
                    }
                }

                return Ok(SuggestedOperations);
            }
        }
    }
}
