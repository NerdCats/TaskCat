using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskCat.Lib.Constants
{
    public class AppConstants
    {
        public const string DefaultApiRoute = "DefaultApi";
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;

        public static readonly string[] SupportedImageFormats = { ".jpg", ".png" };
    }
}