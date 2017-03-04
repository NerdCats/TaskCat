namespace TaskCat.Warehouse.Lib
{
    using Data.Model;

    public class WarehouseOperation
    {
        public string Op { get; internal set; }
        public StockItemModel Payload { get; internal set; }
    }
}
