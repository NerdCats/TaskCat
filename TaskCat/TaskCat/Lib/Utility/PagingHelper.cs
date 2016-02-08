namespace TaskCat.Lib.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http.Routing;

    public class PagingHelper
    {
        UrlHelper urlHelper;
        public PagingHelper(HttpRequestMessage requestMessage)
        {
            urlHelper = new UrlHelper(requestMessage);
        }

        internal string GeneratePageUrl(string route, long page, long pageSize, Dictionary<string, object> otherParams = null)
        {
            Dictionary<string, object> routeParams = new Dictionary<string, object>();
            routeParams.Add("page", page);
            routeParams.Add("pageSize", pageSize);
            routeParams.Add("envelope", true);

            if (otherParams != null)
            {
                foreach (var item in otherParams)
                {
                    routeParams.Add(item.Key, item.Value);
                }
            }

            return urlHelper.Link(route, routeParams);
        }
    }
}