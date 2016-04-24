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
    using Validation;

    public class OrderRepository : IOrderRepository
    {
        JobManager manager;
        SupportedOrderStore supportedOrderStore;
        AccountManager accountManager;
        IHRIDService hridService;
        IOrderCalculationService orderCalculationService;
        IServiceChargeCalculationService serviceChargeCalculationService;
        IOrderValidator orderValidator;

        public OrderRepository(
            JobManager manager, 
            SupportedOrderStore supportedOrderStore, 
            AccountManager accountManager,
            IHRIDService hridService 
            )
        {
            this.manager = manager;
            this.supportedOrderStore = supportedOrderStore;
            this.accountManager = accountManager;
            this.hridService = hridService;
            orderCalculationService = new DefaultOrderCalculationService();
            serviceChargeCalculationService = new DefaultDeliveryServiceChargeCalculationService();
        }

        public async Task<Job> PostOrder(OrderModel model)
        {
            UserModel userModel = new UserModel(await accountManager.FindByIdAsync(model.UserId));

            JobShop jobShop = new JobShop();
            JobBuilder builder;

            switch (model.Type)
            {
                case OrderTypes.Ride:
                    {
                        RideOrder rideOrderModel = model as RideOrder;
                        Validator.ValidateObject(rideOrderModel, new ValidationContext(rideOrderModel), true);
                        builder = new RideJobBuilder(rideOrderModel, userModel, hridService);
                        break;
                    }
                case OrderTypes.Delivery:
                    {
                        DeliveryOrder deliveryOrderModel = model as DeliveryOrder;
                        orderValidator = new DeliveryOrderValidator(orderCalculationService, serviceChargeCalculationService);
                        orderValidator.ValidateOrder(deliveryOrderModel);
                        builder = new DeliveryJobBuilder(deliveryOrderModel, userModel, hridService);
                        break;
                    }
                default:
                    throw new InvalidOperationException("Invalid/Not supported Order Type Provided");

            }
            return await ConstructAndRegister(jobShop, builder);
        }

        public async Task<Job> PostOrder(OrderModel model, string adminUserId)
        {
            UserModel userModel = new UserModel(await accountManager.FindByIdAsync(model.UserId));
            UserModel adminUserModel = new UserModel(await accountManager.FindByIdAsync(adminUserId));

            JobShop jobShop = new JobShop();
            JobBuilder builder;

            switch (model.Type)
            {
                case OrderTypes.Ride:
                    RideOrder rideOrderModel = model as RideOrder;
                    Validator.ValidateObject(rideOrderModel, new ValidationContext(rideOrderModel), true);
                    builder = new RideJobBuilder(rideOrderModel, userModel, adminUserModel, hridService);
                    break;
                case OrderTypes.Delivery:
                    DeliveryOrder deliveryOrderModel = model as DeliveryOrder;
                    Validator.ValidateObject(deliveryOrderModel, new ValidationContext(deliveryOrderModel), true);
                    builder = new DeliveryJobBuilder(deliveryOrderModel, userModel, adminUserModel, hridService);
                    break;
                default:
                    throw new InvalidOperationException("Invalid/Not supported Order Type Provided");

            }
            return await ConstructAndRegister(jobShop, builder);
        }

        private async Task<Job> ConstructAndRegister(JobShop jobShop, JobBuilder builder)
        {
            var createdJob = jobShop.Construct(builder);
            return await manager.RegisterJob(createdJob);
        }

        public async Task<SupportedOrder> PostSupportedOrder(SupportedOrder supportedOrder)
        {
            await supportedOrderStore.Post(supportedOrder);
            return supportedOrder;
        }

        public async Task<List<SupportedOrder>> GetAllSupportedOrder()
        {
            return await supportedOrderStore.GettAll();
        }

        public async Task<SupportedOrder> GetSupportedOrder(string id)
        {
            return await supportedOrderStore.Get(id);
        }

        public async Task<SupportedOrder> UpdateSupportedOrder(SupportedOrder order)
        {
            return await supportedOrderStore.Replace(order);
        }

        public async Task<SupportedOrder> DeleteSupportedOrder(string id)
        {
            return await supportedOrderStore.Delete(id);
        }
    }
}