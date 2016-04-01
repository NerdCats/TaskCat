namespace TaskCat.Lib.Utility
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FileUtility
    {
        public static bool IsFileExtensionSupported(string path, IEnumerable<string> supportedExtensions)
        {
            var extension = Path.GetExtension(path);
            return supportedExtensions.Any(x => extension.EndsWith(x));
        }

        public static bool VerifyFileExtensionSupported(string path, IEnumerable<string> supportedExtensions)
        {
            if (!IsFileExtensionSupported(path, supportedExtensions))
                throw new NotSupportedException(string.Concat("File format not supported, supported formats are: ", string.Join(",", supportedExtensions)));
            return true;
        }
    }
}