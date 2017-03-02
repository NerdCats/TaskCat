namespace TaskCat.Warehouse.Lib
{
    using Data.Entity;
    using Common.Db;
    using MongoDB.Driver;

    public class WarehouseDbContext: DbContext
    {
        private IMongoCollection<StockItem> _stockItems;

        public WarehouseDbContext() : base()
        {
            InitiateWarehouseCollections();
        }

        private void InitiateWarehouseCollections()
        {
            _stockItems = Database.GetCollection<StockItem>(CollectionNames.StockItemCollectionName);
        }
    }
}