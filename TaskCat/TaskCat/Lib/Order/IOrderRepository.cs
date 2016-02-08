namespace TaskCat.Lib.Order
{
    using Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    using System.Web.Http;

    public interface IOrderRepository
    {
        Task<Job> PostOrder(OrderModel model);
        Task<IHttpActionResult> Get(string id);        
    }
}
