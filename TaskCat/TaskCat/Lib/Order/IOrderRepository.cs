namespace TaskCat.Lib.Order
{
    using Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    public interface IOrderRepository
    {
        Task<JobEntity> PostOrder(OrderModel model);
    }
}
