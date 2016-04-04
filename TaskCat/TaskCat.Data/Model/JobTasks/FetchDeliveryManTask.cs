using System;
using TaskCat.Data.Lib.Constants;
using TaskCat.Data.Model.JobTasks.Result;

namespace TaskCat.Data.Model.JobTasks
{
    public class FetchDeliveryManTask : AssignAssetTask
    {
        public FetchDeliveryManTask(Location from, Location to) : base(JobTaskTypes.FETCH_DELIVERYMAN, "Fetching Delivery Guy", from, to)
        {
            this.Result = new DefaultAssignAssetTaskResult();
            State = JobTaskStates.IN_PROGRESS;
        }     
    }
}
