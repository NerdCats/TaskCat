namespace TaskCat.Lib.Order
{
    using Job;
    using Job.Builders;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Entity;
    using Data.Model;
    using Data.Model.Order;
    using System.ComponentModel.DataAnnotations;
    using Auth;
    using Data.Model.Identity.Response;
    using HRID;

    public class OrderRepository : IOrderRepository
    {
        JobManager _manager;
        SupportedOrderStore _supportedOrderStore;
        AccountManager _accountManager;
        IHRIDService _hridService;

        public OrderRepository(
            JobManager manager, 
            SupportedOrderStore supportedOrderStore, 
            AccountManager accountManager,
            IHRIDService hridService 
            )
        {
            _manager = manager;
            _supportedOrderStore = supportedOrderStore;
            _accountManager = accountManager;
            _hridService = hridService;
        }

        public async Task<Job> PostOrder(OrderModel model)
        {
            UserModel userModel = new UserModel(await _accountManager.FindByIdAsync(model.UserId));

            JobShop jobShop = new JobShop();
            JobBuilder builder;

            switch (model.Type)
            {
                case OrderTypes.Ride:
                    RideOrder rideOrderModel = model as RideOrder;
                    Validator.ValidateObject(rideOrderModel, new ValidationContext(rideOrderModel), true);
                    builder = new RideJobBuilder(rideOrderModel, userModel, _hridService);
                    break;
                case OrderTypes.Delivery:
                    DeliveryOrder deliveryOrderModel = model as DeliveryOrder;
                    Validator.ValidateObject(deliveryOrderModel, new ValidationContext(deliveryOrderModel), true);
                    builder = new DeliveryJobBuilder(deliveryOrderModel, userModel, _hridService);
                    break;
                default:
                    throw new InvalidOperationException("Invalid/Not supported Order Type Provided");

            }
            return await ConstructAndRegister(jobShop, builder);
        }

        public async Task<Job> PostOrder(OrderModel model, string adminUserId)
        {
            UserModel userModel = new UserModel(await _accountManager.FindByIdAsync(model.UserId));
            UserModel adminUserModel = new UserModel(await _accountManager.FindByIdAsync(adminUserId));

            JobShop jobShop = new JobShop();
            JobBuilder builder;

            switch (model.Type)
            {
                case OrderTypes.Ride:
                    RideOrder rideOrderModel = model as RideOrder;
                    Validator.ValidateObject(rideOrderModel, new ValidationContext(rideOrderModel), true);
                    builder = new RideJobBuilder(rideOrderModel, userModel, adminUserModel, _hridService);
                    break;
                case OrderTypes.Delivery:
                    DeliveryOrder deliveryOrderModel = model as DeliveryOrder;
                    Validator.ValidateObject(deliveryOrderModel, new ValidationContext(deliveryOrderModel), true);
                    builder = new DeliveryJobBuilder(deliveryOrderModel, userModel, adminUserModel, _hridService);
                    break;
                default:
                    throw new InvalidOperationException("Invalid/Not supported Order Type Provided");

            }
            return await ConstructAndRegister(jobShop, builder);
        }

        private async Task<Job> ConstructAndRegister(JobShop jobShop, JobBuilder builder)
        {
            var createdJob = jobShop.Construct(builder);
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