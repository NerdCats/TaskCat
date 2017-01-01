namespace TaskCat.Data.Model.JobTasks
{
    using Geocoding;
    using Lib.Constants;
    using Result;

    public class FetchDeliveryManTask : AssignAssetTask
    {
        public FetchDeliveryManTask(DefaultAddress from, DefaultAddress to) : base(JobTaskTypes.FETCH_DELIVERYMAN, from, to)
        {
            this.Result = new DefaultAssignAssetTaskResult();
        }     
    }
}
