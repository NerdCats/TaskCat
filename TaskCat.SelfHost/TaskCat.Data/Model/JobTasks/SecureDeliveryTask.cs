namespace TaskCat.Data.Model.JobTasks
{
    using Lib.Constants;
    using Geocoding;
    using System;

    public class SecureDeliveryTask : DeliveryTask
    {

        public override void SetPredecessor(JobTask task, bool validateDependency = true)
        {
            base.SetPredecessor(task, validateDependency);
        }

        public SecureDeliveryTask()
        {
            this.Name = "Secure delivery";
        }

        public SecureDeliveryTask(DefaultAddress from, DefaultAddress to) :
            base(from, to, JobTaskTypes.SECURE_DELIVERY)
        {
            this.Name = "Secure delivery";
        }

        public override void UpdateTask()
        {
            base.UpdateTask();
        }

        public override JobTaskResult SetResultToNextState()
        {
            return base.SetResultToNextState();
        }

        public override string GetHRState()
        {
            var prefix = "Cash delivery to buyer";
            switch (State)
            {
                case JobTaskState.PENDING:
                    return $"{prefix} is pending";
                case JobTaskState.IN_PROGRESS:
                    return $"{prefix} is in progress";
                case JobTaskState.COMPLETED:
                    return $"{prefix} is completed";
                case JobTaskState.CANCELLED:
                    return $"{prefix} is cancelled";
                default:
                    throw new NotImplementedException(State.ToString());
            }
        }
    }
}
