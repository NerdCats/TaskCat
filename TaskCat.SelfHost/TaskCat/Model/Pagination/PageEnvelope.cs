namespace TaskCat.Model.Pagination
{
    using Lib.Utility;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    public class PageEnvelope<T>
    {
        [JsonIgnore]
        public IPagingHelper paginationHelper; 
        public PaginationHeader pagination { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<T> data { get; set; }

        public PageEnvelope(
            long total, 
            long page, 
            int pageSize, 
            string route, 
            IEnumerable<T> data, 
            HttpRequestMessage request)
        {
            var queryParams = request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            paginationHelper = new PagingHelper(request);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);
            var nextPage = page < totalPages - 1 ? paginationHelper.GeneratePageUrl(route, page + 1, pageSize, queryParams) : string.Empty;
            var prevPage = page > 0 ? paginationHelper.GeneratePageUrl(route, page - 1, pageSize, queryParams) : string.Empty;

            this.pagination = new PaginationHeader(total, page, pageSize, data != null ? data.Count() : 0, nextPage, prevPage);
            this.data = data;
        }
    }
}