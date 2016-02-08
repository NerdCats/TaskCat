﻿namespace TaskCat.Lib.Order
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
    using System.Web.Http;

    public class OrderRepository : IOrderRepository
    {
        JobManager _manager;
        public OrderRepository(JobManager manager)
        {
            _manager = manager;
        }

        public Task<IHttpActionResult> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Job> PostOrder(OrderModel model)
        {
            JobShop jobShop = new JobShop();
            Job createdJob;
            switch (model.Type)
            {
                case "Ride":
                    JobBuilder builder = new RideJobBuilder(model as RideOrder);
                    createdJob = jobShop.Construct(builder);
                    break;
                default:
                    throw new InvalidOperationException("Invalid/Not supported Order Type Provided");
                       
            }

            return await _manager.RegisterJob(createdJob);
            
        }
    }
}