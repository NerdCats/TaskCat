namespace TaskCat.Lib.Job.Builders
{
    using Data.Model;
    using Data.Model.Order;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Entity;
    using Data.Model.JobTasks;

    internal class RideJobBuilder : JobBuilder
    {
        private RideOrder _order;
        public RideJobBuilder(RideOrder order) : base(order)
        {
            this._order = order;
        }

        public override void BuildTasks()
        { 
            _job.Tasks = new List<JobTask>();

            //FIXME: I need to check the ride preference and then give a FetchRideTask 
            //according to that
            //Right now just pushing Ryde class as the rest is still not built yet
            FetchRideTask fetchRideTask = new FetchRideTask(_order.From, _order.To,_order.ProposedRide);
            _job.Tasks.Add(fetchRideTask);
            fetchRideTask.AssetUpdated += JobTask_AssetUpdated;
            //FIXME: I really dont know now how would I trigger that would tell which vechicle 
            //got selected or not
            RidePickUpTask pickupRideTask = new RidePickUpTask();
            pickupRideTask.SetPredecessor(fetchRideTask);
            //FIXME: Umm.. this is actually exposing the business logic in a builder.
            _job.Tasks.Add(pickupRideTask);
            pickupRideTask.AssetUpdated += JobTask_AssetUpdated;

            _job.TerminalTask = pickupRideTask;

        }

        // FIXME: I can definitely put this over in the job or at least put a
        // virtual function if just nothing except assigning to the dictionary 
        // happens

        private void JobTask_AssetUpdated(string AssetRef)
        {
            if (!_job.Assets.ContainsKey(AssetRef))
                _job.Assets[AssetRef] = null; // FIXME: I definitely need to fix it here, database fetch needed, dont know how this should work out

        }
    }
}