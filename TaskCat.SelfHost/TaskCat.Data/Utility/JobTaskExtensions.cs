namespace TaskCat.Data.Utility
{
    using Model;
    using System.Text.RegularExpressions;
    using System.Globalization;

    public static class JobTaskExtensions
    {
        public static string GetHrStateString(this JobTask task)
        {
            var variantString = "";

            if (task.Variant != null && !task.Variant.Equals("default"))
            {
                TextInfo variantText = new CultureInfo("en-US", false).TextInfo;
                variantString = variantText.ToTitleCase(task.Variant);
            }
            
            var stateString = task.State.ToString().Replace("_", " ").ToLowerInvariant();
            //Regex to detect the capital letter as a starting of a word and insert an space between them 
            var presentTask = Regex.Replace(task.Type, "[A-Z]", " $0").Trim();

            if (task.Variant.Equals("return"))
            {
                presentTask = "";
                return $"Package {variantString.ToLower()} {stateString}";
            }
            return $"{variantString} {presentTask} {stateString}".Trim();
        }

        public static bool IsConclusiveStateToMoveToNextTask(this JobTaskState state)
        {
            var conclusiveState = JobTaskState.FAILED | JobTaskState.RETURNED | JobTaskState.COMPLETED;
            var result = conclusiveState & state;
            return result == state;
        }
    }
}
