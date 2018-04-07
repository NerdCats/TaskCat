using System;
using TaskCat.Data.Model;
using TaskCat.PartnerModels.Infini;

namespace TaskCat.BackgroundJobService
{
    internal class StatusTranslator
    {
        internal static OrderStatusCode TranslateStatus(string remoteJobState)
        {
            if (remoteJobState == null)
            {
                throw new ArgumentNullException(nameof(remoteJobState));
            }

            switch(remoteJobState)
            {
                case var jobState when (jobState == JobState.COMPLETED.ToString()):
                    return OrderStatusCode.Delivered;
                case var jobState when (jobState == JobState.CANCELLED.ToString()):
                    return OrderStatusCode.Cancelled;
                case var jobState when (jobState == JobState.IN_PROGRESS.ToString()):
                    return OrderStatusCode.Undelivered;
                case var jobState when (jobState == JobTaskState.RETURNED.ToString()):
                    return OrderStatusCode.Returned;
                default:
                    return OrderStatusCode.Undefined;
            }
        }
    }
}