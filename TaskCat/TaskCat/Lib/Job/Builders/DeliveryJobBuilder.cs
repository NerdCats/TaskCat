namespace TaskCat.Lib.Job.Builders
{
    using Data.Model;
    using Data.Model.JobTasks;
    using Data.Model.Order;
    using System.Collections.Generic;
    using Data.Model.Identity.Response;
    using Model.JobTasks;
    internal class DeliveryJobBuilder : JobBuilder
    {
        private DeliveryOrder _order;
        public DeliveryJobBuilder(DeliveryOrder order) : base(order)
        {
            this._order = order;
        }

        public override void BuildTasks()
        {
            //FIXME: Looks like I can definitely refactor this and work this out

            _job.Tasks = new List<JobTask>();

            FetchRideTask fetchRideTask = new FetchRideTask(_order.From, _order.To);
            _job.Tasks.Add(fetchRideTask);
            fetchRideTask.AssetUpdated += JobTask_AssetUpdated;

            PackagePickUpTask pickUpTask = new PackagePickUpTask(_order.From);
            pickUpTask.SetPredecessor(fetchRideTask);
            _job.Tasks.Add(pickUpTask);
            pickUpTask.AssetUpdated += JobTask_AssetUpdated;

            DeliveryTask deliveryTask = new DeliveryTask(_order.From, _order.To);
            deliveryTask.SetPredecessor(pickUpTask);
            _job.Tasks.Add(deliveryTask);
            deliveryTask.AssetUpdated += JobTask_AssetUpdated;

            _job.TerminalTask = deliveryTask;

            _job.EnsureTaskAssetEventsAssigned();
        }

   

        private void JobTask_AssetUpdated(string AssetRef, AssetModel asset)
        {
            //FIXME: Replicating code constantly, need to fix these
            if (!_job.Assets.ContainsKey(AssetRef))
                _job.Assets[AssetRef] = asset; 
        }
    }
}