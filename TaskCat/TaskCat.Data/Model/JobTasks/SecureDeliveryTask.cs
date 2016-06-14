namespace TaskCat.Data.Model.JobTasks
{
    using Lib.Constants;
    using Geocoding;

    public class SecureDeliveryTask : DeliveryTask
    {
        public SecureDeliveryTask(DefaultAddress from, DefaultAddress to) : 
            base(from, to, JobTaskTypes.SECURE_DELIVERY, "Deliverying Secure Package")
        {
        }
    }

}
