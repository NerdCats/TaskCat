namespace TaskCat.Job
{
    using System.Collections.Generic;

    public static class JobTaskExtensionResolver
    {
        private static Dictionary<string, List<JobTaskExtension>> extensionsDictionary
            = new Dictionary<string, List<JobTaskExtension>>();

        public static void Register(string orderType, List<JobTaskExtension> extensions)
        {
            if (extensionsDictionary.ContainsKey(orderType))
            {
                extensionsDictionary[orderType].AddRange(extensions);
            }
            else
            {
                extensionsDictionary.Add(orderType, extensions);
            }
        }

        public static List<JobTaskExtension> Resolve(string orderType)
        {
            if (!extensionsDictionary.ContainsKey(orderType))
                return null;
            else
                return extensionsDictionary[orderType];
        }
    }
}
