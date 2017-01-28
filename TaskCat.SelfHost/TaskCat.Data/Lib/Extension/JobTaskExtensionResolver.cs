﻿namespace TaskCat.Data.Lib.Extension
{
    using System.Collections.Generic;
    using System.Linq;

    public static class JobTaskExtensionResolver
    {
        private static Dictionary<string, List<JobTaskExtension>> extensionsDictionary
            = new Dictionary<string, List<JobTaskExtension>>();

        private static bool isInitialized = false;

        public static void Register(string orderType, List<JobTaskExtension> extensions)
        {
            if (!isInitialized)
            {
                if (extensionsDictionary.ContainsKey(orderType))
                {
                    extensionsDictionary[orderType].AddRange(extensions);
                }
                else
                {
                    extensionsDictionary.Add(orderType, extensions);
                }

                isInitialized = true;
            }
        }

        public static List<JobTaskExtension> Resolve(string orderType, string taskType)
        {
            if (!extensionsDictionary.ContainsKey(orderType))
                return null;
            else
            {
                var extensions = extensionsDictionary[orderType];
                return extensions.Where(x => x.JobTaskType == taskType).ToList();
            }
        }
    }
}
