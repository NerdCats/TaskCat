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
        //FIXME: We might need a url generator, this is plain ghetto
        internal string GenerateNextPageUrl(string absolutePath, long start, long limit, long total, Dictionary<string, object> otherParams = null)
        {
            if (start + limit == total || total == 0)
                return string.Empty;

            limit = start + limit > total ? total : limit;

            Dictionary<string, object> routeParams = new Dictionary<string, object>();
            routeParams.Add("start", start + limit);
            routeParams.Add("limit", limit);
            routeParams.Add("envelope", true);

            if (otherParams != null)
            {
                foreach (var item in otherParams)
                {
                    routeParams.Add(item.Key, item.Value);
                }
            }

            return urlHelper.Link(absolutePath, routeParams);
        }

        internal string GeneratePreviousPageUrl(string absolutePath, long start, long limit, long total, Dictionary<string, object> otherParams = null)
        {
            if (start == 0)
                return string.Empty;

            limit = start - 1 < 0 ? 0 : start - 1;
            start = start - limit - 1 < 0 ? 0 : start - limit - 1;


            Dictionary<string, object> routeParams = new Dictionary<string, object>();
            routeParams.Add("start", start);
            routeParams.Add("limit", limit);
            routeParams.Add("envelope", true);

            if (otherParams != null)
            {
                foreach (var item in otherParams)
                {
                    routeParams.Add(item.Key, item.Value);
                } 
            }

            return urlHelper.Link(absolutePath, routeParams);
        }
    }
}