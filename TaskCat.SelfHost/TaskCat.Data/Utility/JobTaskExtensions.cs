namespace TaskCat.Data.Utility
{
    using Model;
    using System.Text.RegularExpressions;

    public static class JobTaskExtensions
    {
        public static string GetHrStateString(this JobTask task)
        {
            var stateString = task.State.ToString().Replace("_", " ").ToLowerInvariant();
            //Regex to detect the capital letter as a starting of a word and insert an space between them 
            var presentTask = Regex.Replace(task.Type, "[A-Z]", " $0").Trim();
            return $"{presentTask} {stateString}";
        }

        public static bool IsConclusiveStateToMoveToNextTask(this JobTaskState state)
        {
            var conclusiveState = JobTaskState.FAILED | JobTaskState.RETURNED | JobTaskState.COMPLETED;
            var result = conclusiveState & state;
            return result == state;
        }
    }
}
