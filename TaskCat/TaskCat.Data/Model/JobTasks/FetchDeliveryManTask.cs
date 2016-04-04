namespace TaskCat.Data.Model.JobTasks
{
    using Lib.Constants;
    using Result;

    public class FetchDeliveryManTask : AssignAssetTask
    {
        public FetchDeliveryManTask(Location from, Location to) : base(JobTaskTypes.FETCH_DELIVERYMAN, "Fetching Delivery Guy", from, to)
        {
            this.Result = new DefaultAssignAssetTaskResult();
            State = JobTaskStates.IN_PROGRESS;
        }     
    }
}
