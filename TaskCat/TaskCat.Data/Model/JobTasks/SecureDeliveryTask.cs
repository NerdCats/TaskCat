using TaskCat.Data.Lib.Constants;
using TaskCat.Data.Model.Geocoding;

namespace TaskCat.Data.Model.JobTasks
{
    public class SecureDeliveryTask : DeliveryTask
    {
        public SecureDeliveryTask(DefaultAddress from, DefaultAddress to) : 
            base(from, to, JobTaskTypes.SECURE_DELIVERY, "Deliverying Secure Package")
        {
        }
    }

}
