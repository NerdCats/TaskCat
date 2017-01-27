namespace TaskCat.Data.Utility
{
    using Model;

    public static class JobTaskExtensions
    {
        public static string GetHrStateString(this JobTask task)
        {
            var stateString = task.State.ToString().Replace("_", " ").ToLowerInvariant();
            return $"{task.Type} {stateString}";
        }

        public static bool IsConclusiveStateToMoveToNextTask(this JobTaskState state)
        {
            var conclusiveState = JobTaskState.FAILED | JobTaskState.RETURNED | JobTaskState.COMPLETED;
            var result = conclusiveState & state;
            return result == state;
        }
    }
}
