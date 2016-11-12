namespace TaskCat.Lib.Utility.ActionFilter
{
    using Constants;
    using Odata;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Threading;
    using System.Threading.Tasks;
    using LinqToQuerystring;
    using System.Net;
    using System.Net.Http.Formatting;
    using System;

    public class TaskCatOdataRouteAttribute : ActionFilterAttribute
    {
        public OdataRequestModel odataRequestModel { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var queryParams = request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);

            int pageSize = AppConstants.DefaultPageSize;
            int page = 0;
            bool envelope = true;
            bool countOnly = false;

            if (queryParams.ContainsKey(PagingQueryParameters.Page))
                int.TryParse(queryParams[PagingQueryParameters.Page], out page);

            if (queryParams.ContainsKey(PagingQueryParameters.PageSize))
                int.TryParse(queryParams[PagingQueryParameters.PageSize], out pageSize);

            pageSize = PagingHelper.ValidatePageSize(AppConstants.MaxPageSize, pageSize, page);

            if (queryParams.ContainsKey(PagingQueryParameters.Envelope))
                bool.TryParse(queryParams[PagingQueryParameters.Envelope], out envelope);

            if (queryParams.ContainsKey(PagingQueryParameters.CountOnly))
                countOnly = true;

            var odataQuery = request.GetOdataQueryString(PagingQueryParameters.DefaultPagingParams);

            request.Properties["OdataQueryString"] = odataQuery;
            request.Properties[PagingQueryParameters.Envelope] = envelope;
            request.Properties[PagingQueryParameters.Page] = page;
            request.Properties[PagingQueryParameters.PageSize] = pageSize;
            request.Properties[PagingQueryParameters.CountOnly] = countOnly;

            odataRequestModel = new OdataRequestModel()
            {
                Envelope = envelope,
                OdataQueryString = odataQuery,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
