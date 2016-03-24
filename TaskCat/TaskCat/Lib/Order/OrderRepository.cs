namespace TaskCat.Lib.Order
{
    using Job;
    using Job.Builders;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;
    using TaskCat.Data.Model;
    using Data.Model.Order;
    using Order;

    public class OrderRepository : IOrderRepository
    {
        JobManager _manager;
        SupportedOrderStore _supportedOrderStore;
        public OrderRepository(JobManager manager, SupportedOrderStore supportedOrderStore)
        {
            _manager = manager;
            _supportedOrderStore = supportedOrderStore;
        }

        public async Task<Job> PostOrder(OrderModel model)
        {
            JobShop jobShop = new JobShop();
            JobBuilder builder;
            Job createdJob;
            switch (model.Type)
            {
                case "Ride":
                    builder = new RideJobBuilder(model as RideOrder);
                    createdJob = jobShop.Construct(builder);
                    break;
                case "Delivery":
                    builder = new DeliveryJobBuilder(model as DeliveryOrder);
                    createdJob = jobShop.Construct(builder);
                    break;
                default:
                    throw new InvalidOperationException("Invalid/Not supported Order Type Provided");

            }

            return await _manager.RegisterJob(createdJob);       
        }

        public async Task<SupportedOrder> PostSupportedOrder(SupportedOrder supportedOrder)
        {
            await _supportedOrderStore.Post(supportedOrder);
            return supportedOrder;
        }

        public async Task<List<SupportedOrder>> GetAllSupportedOrder()
        {
            return await _supportedOrderStore.GettAll();
        }

        public async Task<SupportedOrder> GetSupportedOrder(string id)
        {
            return await _supportedOrderStore.Get(id);
        }

        public async Task<SupportedOrder> UpdateSupportedOrder(SupportedOrder order)
        {
            return await _supportedOrderStore.Replace(order);
        }

        public async Task<SupportedOrder> DeleteSupportedOrder(string id)
        {
            return await _supportedOrderStore.Delete(id);
        }
    }
}