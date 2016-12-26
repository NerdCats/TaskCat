namespace TaskCat.Job.Updaters
{
    public class JobUpdateMode
    {
        public const string smart = "smart";
        public const string force = "force";

        public static bool IsValidUpdateMode(string updateMode)
        {
            return updateMode == smart || updateMode == force;
        }
    }
}
