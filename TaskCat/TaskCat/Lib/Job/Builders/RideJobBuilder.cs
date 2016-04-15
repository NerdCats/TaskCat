namespace TaskCat.Lib.Job.Builders
{
    using Data.Model;
    using Data.Model.Order;
    using System.Collections.Generic;
    using Data.Model.JobTasks;
    using Data.Model.Identity.Response;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class RideJobBuilder : JobBuilder
    {
        private RideOrder _order;
        public RideJobBuilder(RideOrder order, UserModel userModel) : base(order, userModel)
        {
            this._order = order;
        }

        public RideJobBuilder(RideOrder order, UserModel userModel, UserModel adminUserModel) : base(order, userModel, adminUserModel)
        {
            this._order = order;
        }

        public override void BuildTasks()
        { 
            job.Tasks = new List<JobTask>();

            //FIXME: I need to check the ride preference and then give a FetchRideTask 
            //according to that
            //Right now just pushing Ryde class as the rest is still not built yet
            FetchRideTask fetchRideTask = new FetchRideTask(_order.From, _order.To,_order.ProposedRide);
            job.Tasks.Add(fetchRideTask);
            fetchRideTask.AssetUpdated += JobTask_AssetUpdated;
            //FIXME: I really dont know now how would I trigger that would tell which vechicle 
            //got selected or not
            RidePickUpTask pickupRideTask = new RidePickUpTask(_order.From);
            pickupRideTask.SetPredecessor(fetchRideTask);
            //FIXME: Umm.. this is actually exposing the business logic in a builder.
            job.Tasks.Add(pickupRideTask);
            pickupRideTask.AssetUpdated += JobTask_AssetUpdated;

            job.TerminalTask = pickupRideTask;

            job.EnsureTaskAssetEventsAssigned();

        }

        // FIXME: I can definitely put this over in the job or at least put a
        // virtual function if just nothing except assigning to the dictionary 
        // happens

        private void JobTask_AssetUpdated(string AssetRef, AssetModel asset)
        {
            if (!job.Assets.ContainsKey(AssetRef))
                job.Assets[AssetRef] = asset; // FIXME: I definitely need to fix it here, database fetch needed, dont know how this should work out

        }
    }
}