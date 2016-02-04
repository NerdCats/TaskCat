namespace TaskCat.Data.Model.Pagination
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PageEnvelope<T> 
    {
        public PaginationHeader pagination { get; set; }
        public IEnumerable<T> data { get; set; }

        public PageEnvelope(PaginationHeader paginationHeader, IEnumerable<T> data)
        {
            this.pagination = paginationHeader;
            this.data = data;
        }
    }
}
