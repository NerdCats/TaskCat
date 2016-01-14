namespace TaskCat.Lib.Order
{
    using Data.Model.Order;
    using Job;
    using Job.Builders;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;
    using TaskCat.Data.Model;

    public class OrderRepository : IOrderRepository
    {
        public OrderRepository()
        {
            
        }
        public Task<JobEntity> PostOrder(OrderModel model)
        {
            switch (model.Type)
            {
                case "Ride":
                    JobBuilder builder = new RideJobBuilder(model as RideOrder);
                    break;
                    
                       
            }

            throw new NotImplementedException();

            
        }
    }
}