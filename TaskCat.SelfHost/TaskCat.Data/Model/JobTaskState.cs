using System;

namespace TaskCat.Data.Model
{
    [Flags]
    public enum JobTaskState
    {
        /// <summary>
        /// Task has not started yet
        /// </summary>
        PENDING = 1,
        /// <summary>
        /// Task is in progress
        /// </summary>
        IN_PROGRESS = 2,
        /// <summary>
        /// Task is completed
        /// </summary>
        COMPLETED = 4,
        /// <summary>
        /// Task is cancelled, no default operation to follow.
        /// Incerementing attempt should be default.
        /// </summary>
        CANCELLED = 8,
        /// <summary>
        /// Task is returned, default operation is to return product to the sender.
        /// </summary>
        RETURNED = 16,
        /// <summary>
        /// Task has failed, deafult operation is to increment the attempt.
        /// Can be customized for an extending task.
        /// </summary>
        FAILED = 32
    }
}
