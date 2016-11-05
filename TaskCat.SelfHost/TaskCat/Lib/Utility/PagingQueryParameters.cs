namespace TaskCat.Lib.Utility
{
    using System.Collections.Generic;

    public class PagingQueryParameters
    {
        public const string PageSize = "pageSize";
        public const string Page = "page";
        public const string Envelope = "envelope";

        public static List<string> DefaultPagingParams = 
            new List<string>()
            {
                PageSize,
                Page,
                Envelope
            };
    }
}
