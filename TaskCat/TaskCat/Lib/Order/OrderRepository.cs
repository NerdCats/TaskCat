﻿namespace TaskCat.Lib.Order
{
    using Job;
    using Job.Builders;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Data.Entity;
    using TaskCat.Data.Model;
    using Data.Model.Order;
    using System.ComponentModel.DataAnnotations;
    using Auth;
    using Data.Model.Identity.Response;

    public class OrderRepository : IOrderRepository
    {
        JobManager _manager;
        SupportedOrderStore _supportedOrderStore;
        AccountManager _accountManager;

        public OrderRepository(JobManager manager, SupportedOrderStore supportedOrderStore, AccountManager accountManager)
        {
            _manager = manager;
            _supportedOrderStore = supportedOrderStore;
            _accountManager = accountManager;
        }

        public async Task<Job> PostOrder(OrderModel model)
        {
            if (model.User == "Anonymous" || string.IsNullOrWhiteSpace(model.User))
                    throw new ArgumentException("User came off as Anonymous or null");

            UserModel userModel = new UserModel(await _accountManager.FindByIdAsync(model.User));

            JobShop jobShop = new JobShop();
            JobBuilder builder;
            Job createdJob;
            switch (model.Type)
            {
                case OrderTypes.Ride:
                    RideOrder rideOrderModel = model as RideOrder;
                    Validator.ValidateObject(rideOrderModel, new ValidationContext(rideOrderModel), true);
                    builder = new RideJobBuilder(model as RideOrder, userModel); 
                    break;
                case OrderTypes.Delivery:
                    DeliveryOrder deliveryOrderModel = model as DeliveryOrder;
                    Validator.ValidateObject(deliveryOrderModel, new ValidationContext(deliveryOrderModel), true);
                    builder = new DeliveryJobBuilder(deliveryOrderModel, userModel);             
                    break;
                default:
                    throw new InvalidOperationException("Invalid/Not supported Order Type Provided");

            }
            createdJob = jobShop.Construct(builder);

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