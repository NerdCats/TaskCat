namespace TaskCat.Model.Pagination
{
    using Lib.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class PageEnvelope<T>
    {
        private PagingHelper _paginationHelper; 
        public PaginationHeader pagination { get; set; }
        public IEnumerable<T> data { get; set; }

        public PageEnvelope(
            long total, 
            long page, 
            int pageSize, 
            string route, 
            IEnumerable<T> data, 
            HttpRequestMessage request,
            Dictionary<string, object> otherParams = null)
        {
            _paginationHelper = new PagingHelper(request);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);
            var nextPage = page < totalPages - 1 ? _paginationHelper.GeneratePageUrl(route, page + 1, pageSize, otherParams) : string.Empty;
            var prevPage = page > 0 ? _paginationHelper.GeneratePageUrl(route, page, pageSize, otherParams) : string.Empty;

            this.pagination = new PaginationHeader(total, page, pageSize, data.Count(), nextPage, prevPage);
            this.data = data;
        }
    }
}