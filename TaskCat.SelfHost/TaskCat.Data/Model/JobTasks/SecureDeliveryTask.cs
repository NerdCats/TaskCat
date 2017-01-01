namespace TaskCat.Data.Model.JobTasks
{
    using Lib.Constants;
    using Geocoding;

    public class SecureDeliveryTask : DeliveryTask
    {

        public override void SetPredecessor(JobTask task, bool validateDependency = true)
        {
            base.SetPredecessor(task, validateDependency);
        }

        public SecureDeliveryTask(DefaultAddress from, DefaultAddress to) : 
            base(from, to, JobTaskTypes.SECURE_DELIVERY)
        {
        }

        public override void UpdateTask()
        {
            base.UpdateTask();
        }

        public override JobTaskResult SetResultToNextState()
        {
            return base.SetResultToNextState();
        }
    }

}
