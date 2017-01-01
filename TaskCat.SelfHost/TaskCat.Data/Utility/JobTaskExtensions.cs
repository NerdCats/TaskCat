namespace TaskCat.Data.Utility
{
    using Lib.Constants;
    using Model;
    using System.Globalization;

    public static class JobTaskExtensions
    {
        public static string GetHrState(this JobTask task)
        {
            var stateString = task.State.ToString().Replace("_", " ").ToLowerInvariant();
            return $"{task.Type} {stateString}";
        }
    }
}
