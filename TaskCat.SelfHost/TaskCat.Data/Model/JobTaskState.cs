using System;
using System.ComponentModel;

namespace TaskCat.Data.Model
{
    [Flags]
    [DefaultValue(PENDING)]
    public enum JobTaskState
    {
        /// <summary>
        /// Task has not started yet
        /// </summary>
        PENDING = 0,
        /// <summary>
        /// Task is in progress
        /// </summary>
        IN_PROGRESS = 1,
        /// <summary>
        /// Task is completed
        /// </summary>
        COMPLETED = 2,
        /// <summary>
        /// Task is cancelled, no default operation to follow.
        /// Incerementing attempt should be default.
        /// </summary>
        CANCELLED = 4,
        /// <summary>
        /// Task is returned, default operation is to return product to the sender.
        /// </summary>
        RETURNED = 8,
        /// <summary>
        /// Task has failed, deafult operation is to increment the attempt.
        /// Can be customized for an extending task.
        /// </summary>
        FAILED = 16
    }
}
