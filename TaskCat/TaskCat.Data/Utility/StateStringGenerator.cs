namespace TaskCat.Data.Utility
{
    using Data.Model;
    using System;

    public class StateStringGenerator
    {
        public static string GenerateStateString(JobTaskState state, string JobTaskName)
        {
            switch(state)
            {
                case JobTaskState.PENDING:
                    return string.Concat(JobTaskName, " is pending");
                case JobTaskState.IN_PROGRESS:
                    return string.Concat(JobTaskName, " is in progress");
                case JobTaskState.COMPLETED:
                    return string.Concat(JobTaskName, " is completed");
                default:
                    throw new InvalidOperationException("Invalid/Unsupported JobTask state provided");
            }
        }

        public static string GenerateStateString(JobState state, string JobName)
        {
            switch (state)
            {
                case JobState.ENQUEUED:
                    return string.Concat(JobName, " is enqueued");
                case JobState.IN_PROGRESS:
                    return string.Concat(JobName, " is in progress");
                case JobState.COMPLETED:
                    return string.Concat(JobName, " is completed");
                default:
                    throw new InvalidOperationException("Invalid/Unsupported Job state provided");
            }
        }
    }
}