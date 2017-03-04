using System.Threading.Tasks;
using MongoDB.Driver;
using TaskCat.Data.Entity;
using System.Collections.Generic;

namespace TaskCat.Warehouse.Lib
{
    public interface IStockService
    {
        IMongoCollection<StockItem> Collection { get; }

        Task<StockItem> Delete(string id);
        Task<StockItem> Get(string id);
        Task<StockItem> Insert(StockItem obj);
        Task<StockItem> Update(StockItem obj);

        Task<IEnumerable<StockItem>> GetStocksByReference(string referenceId, string refType);
    }
}