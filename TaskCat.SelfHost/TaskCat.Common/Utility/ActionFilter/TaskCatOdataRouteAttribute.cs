namespace TaskCat.Common.Utility.ActionFilter
{
    using Odata;
    using Paging;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public class TaskCatOdataRouteAttribute : ActionFilterAttribute
    {
        private int maxPageSize;

        public TaskCatOdataRouteAttribute(int maxPageSize)
        {
            this.maxPageSize = maxPageSize;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var queryParams = request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

            int pageSize = maxPageSize;
            int page = 0;
            bool envelope = true;
            bool countOnly = false;
            bool containsSelect = false;

            if (queryParams.ContainsKey(PagingQueryParameters.Page))
                int.TryParse(queryParams[PagingQueryParameters.Page], out page);

            if (queryParams.ContainsKey(PagingQueryParameters.PageSize))
                int.TryParse(queryParams[PagingQueryParameters.PageSize], out pageSize);

            pageSize = PagingHelper.ValidatePageSize(maxPageSize, pageSize, page);

            if (queryParams.ContainsKey(PagingQueryParameters.Envelope))
                bool.TryParse(queryParams[PagingQueryParameters.Envelope], out envelope);

            if (queryParams.ContainsKey(PagingQueryParameters.CountOnly))
                countOnly = true;

            if (queryParams.ContainsKey("$select"))
                containsSelect = true;

            var odataQuery = request.GetOdataQueryString(PagingQueryParameters.DefaultPagingParams);

            request.Properties["OdataQueryString"] = odataQuery;
            request.Properties[PagingQueryParameters.Envelope] = envelope;
            request.Properties[PagingQueryParameters.Page] = page;
            request.Properties[PagingQueryParameters.PageSize] = pageSize;
            request.Properties[PagingQueryParameters.CountOnly] = countOnly;
            request.Properties["ContainsSelect"] = containsSelect;
        }
    }
}
