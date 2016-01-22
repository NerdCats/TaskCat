namespace TaskCat.Lib.Job.Builders
{
    using Data.Entity.Assets;
    using Data.Model;
    using Data.Model.Order;
    using Model.JobTasks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Entity;
    public class RideJobBuilder : JobBuilder
    {
        private RideOrder _order;
        public RideJobBuilder(RideOrder order) : base(order.Name)
        {
            this._order = order;
        }

        public override void BuildTasks()
        { 
            _job.Tasks = new List<JobTask>();

            //FIXME: I need to check the ride preference and then give a FetchRideTask 
            //according to that
            //Right now just pushing Ryde class as the rest is still not built yet
            FetchRideTask<Ride> fetchRideTask = new FetchRideTask<Ride>(_order.From, _order.To,_order.ProposedRide);
            _job.Tasks.Add(fetchRideTask);
            //FIXME: I really dont know now how would I trigger that would tell which vechicle 
            //got selected or not
            RidePickUpTask pickupRideTask = new RidePickUpTask();
            //FIXME: Umm.. this is actually exposing the business logic in a builder.
            _job.Tasks.Add(pickupRideTask);

            _job.TerminalTask = pickupRideTask;

        }


    }
}