namespace TaskCat.Lib.Utility
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http.Routing;

    public class PagingHelper
    {
        UrlHelper urlHelper;
        public PagingHelper(HttpRequestMessage requestMessage)
        {
            urlHelper = new UrlHelper(requestMessage);
        }

        internal string GeneratePageUrl(string route, long page, long pageSize, IDictionary<string, string> otherParams = null)
        {
            Dictionary<string, object> routeParams = new Dictionary<string, object>();
            routeParams.Add("page", page);
            routeParams.Add("pageSize", pageSize);
            routeParams.Add("envelope", true);

            if (otherParams != null)
            {
                foreach (var item in otherParams)
                {
                    if (!routeParams.ContainsKey(item.Key))
                        routeParams.Add(item.Key, item.Value);
                }
            }

            return urlHelper.Link(route, routeParams);
        }

    }
}