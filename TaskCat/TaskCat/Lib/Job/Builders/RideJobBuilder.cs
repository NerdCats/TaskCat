namespace TaskCat.Lib.Job.Builders
{
    using Data.Entity.JobTasks;
    using Data.Model;
    using Data.Model.Order;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Entity;
    public class RideJobBuilder : JobBuilder
    {
        private OrderModel _order;
        public RideJobBuilder(OrderModel order) : base(order.Name)
        {
            // Although it might look like that I shouldve used
            // just RideOrderModel to construct this RideJobBuilder
            // Some might create a different payload for ride, so
            // I kept the wront order type exception this way
            if (order.Type != "Ride")
                throw new InvalidOperationException("Order of Ride Type Expected");

            this._order = order;
        }

        public override void BuildTaks()
        { 
            _job.Tasks = new List<JobTask>();

            FetchRideTask fetchRideTask = new FetchRideTask();
        }


    }
}