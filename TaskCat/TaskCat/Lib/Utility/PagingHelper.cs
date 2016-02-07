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
        internal  string GenerateNextPageUrl(string absolutePath, string type, long start, long limit, long total)
        {
            if (start + limit == total || total == 0)
                return string.Empty;

            limit = start + limit > total ? total : limit;

            return urlHelper.Link(absolutePath, new
            {
                type = type,
                start = start + limit,
                limit = limit,
                envelope = true
            });
        }

        internal  string GeneratePreviousPageUrl(string absolutePath, string type, long start, long limit, long total)
        {
            if (start == 0)
                return string.Empty;

            limit = start - 1 < 0 ? 0 : start-1;
            start = start - limit - 1 < 0 ? 0 : start - limit - 1;
            return urlHelper.Link(absolutePath, new
            {
                type = type,
                start = start,
                limit = limit,
                envelope = true
            });
        }
    }
}