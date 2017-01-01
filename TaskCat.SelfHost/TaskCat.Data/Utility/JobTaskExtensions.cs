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
    }
}
