namespace TaskCat.Lib.Order
{
    using Data.Entity;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;

    public interface IOrderRepository
    {
        Task<Job> PostOrder(OrderModel model);
        Task<SupportedOrder> PostSupportedOrder(SupportedOrder supportedOrder);
        Task<List<SupportedOrder>> GetAllSupportedOrder();
        Task<SupportedOrder> GetSupportedOrder(string id);
        Task<SupportedOrder> UpdateSupportedOrder(SupportedOrder order);
        Task<SupportedOrder> DeleteSupportedOrder(string id);
    }
}
