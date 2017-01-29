namespace TaskCat.Data.Utility
{
    using Model;
    using System.Text.RegularExpressions;

    public static class JobTaskExtensions
    {
        public static string GetHrStateString(this JobTask task)
        {
            var stateString = task.State.ToString().Replace("_", " ").ToLowerInvariant();
            var presentTask = Regex.Replace(task.Type, "[A-Z]", " $0").Trim();
            return $"{presentTask} {stateString}";
        }
    }
}
