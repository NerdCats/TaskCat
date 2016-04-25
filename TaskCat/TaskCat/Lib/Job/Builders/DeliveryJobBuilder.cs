namespace TaskCat.Lib.Job.Builders
{
    using Data.Model;
    using Data.Model.JobTasks;
    using Data.Model.Order;
    using System.Collections.Generic;
    using Data.Model.Identity.Response;
    using HRID;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DeliveryJobBuilder : JobBuilder
    {
        private DeliveryOrder _order;
        public DeliveryJobBuilder(DeliveryOrder order, UserModel userModel, IHRIDService hridService) : base(order, userModel,hridService)
        {
            this._order = order;
        }

        public DeliveryJobBuilder(DeliveryOrder order, UserModel userModel, UserModel adminUserModel, IHRIDService hridService) : base(order, userModel, adminUserModel, hridService)
        {
            this._order = order;
        }

        public override void BuildTasks()
        {
            //FIXME: Looks like I can definitely refactor this and work this out
            
            job.Tasks = new List<JobTask>();

            FetchDeliveryManTask fetchDeliveryManTask = new FetchDeliveryManTask(_order.From, _order.To);
            job.Tasks.Add(fetchDeliveryManTask);         
            fetchDeliveryManTask.AssetUpdated += JobTask_AssetUpdated;

            PackagePickUpTask pickUpTask = new PackagePickUpTask(_order.From);
            pickUpTask.SetPredecessor(fetchDeliveryManTask);
            job.Tasks.Add(pickUpTask);
            pickUpTask.AssetUpdated += JobTask_AssetUpdated;

            DeliveryTask deliveryTask = new DeliveryTask(_order.From, _order.To);
            deliveryTask.SetPredecessor(pickUpTask);
            job.Tasks.Add(deliveryTask);
            deliveryTask.AssetUpdated += JobTask_AssetUpdated;

            job.TerminalTask = deliveryTask;

            job.EnsureTaskAssetEventsAssigned();
            job.EnsureInitialJobState();

            job.SetupDefaultBehaviourForFirstJobTask();
        }


        private void JobTask_AssetUpdated(string AssetRef, AssetModel asset)
        {
            //FIXME: Replicating code constantly, need to fix these
            if (!job.Assets.ContainsKey(AssetRef))
                job.Assets[AssetRef] = asset; 
        }
    }
}