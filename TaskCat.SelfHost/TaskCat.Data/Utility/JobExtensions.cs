namespace TaskCat.Data.Utility
{
    using System.Globalization;
    using Model;

    public static class JobExtensions
    {
        public static string GetSimplifiedStateString(this JobState state)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(state.ToString().ToLowerInvariant());
        }
    }
}
