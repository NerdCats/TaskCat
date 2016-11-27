namespace TaskCat.Common.Utility
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public static class UrlExtensions
    {
        public static string ToQuerystring(this IDictionary<string, string> queryParams)
        {
            var paramPairs = queryParams.Select(x => $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value)}");
            return string.Concat("?", string.Join("&", paramPairs.ToArray()));
        } 
    }
}