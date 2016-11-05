namespace TaskCat.Lib.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http.Routing;

    public class PagingHelper : IPagingHelper
    {
        UrlHelper urlHelper;
        public PagingHelper(HttpRequestMessage requestMessage)
        {
            urlHelper = new UrlHelper(requestMessage);
        }

        public virtual string GeneratePageUrl(string route, long page, long pageSize, IDictionary<string, string> otherParams = null)
        {
            Dictionary<string, object> routeParams = new Dictionary<string, object>();
            routeParams.Add(PagingQueryParameters.Page, page);
            routeParams.Add(PagingQueryParameters.PageSize, pageSize);
            routeParams.Add(PagingQueryParameters.Envelope, true);

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

        public static int ValidatePageSize(int maxPageSize, int pageSize, int page)
        {
            if (pageSize == 0)
                throw new ArgumentException("Page size cant be 0");

            if (page < 0)
                throw new ArgumentException("Page index less than 0 provided");

            pageSize = pageSize > maxPageSize ? maxPageSize : pageSize;
            return pageSize;
        }
    }
}