namespace TaskCat.Model.Pagination
{
    using System;
    public class PaginationHeader
    {
        public long Total { get; set; }
        public long Page { get; set; }
        public long Start { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int Returned { get; set; }
        public string NextPage { get; set; }
        public string PrevPage { get; set; }

        public PaginationHeader(long total, long page, int pageSize, int returned, string NextPage, string PrevPage)
        {
            this.Total = total;
            this.Page = page;
            this.Start = page * pageSize + 1;
            this.PageSize = pageSize;
            this.Returned = returned;
            this.TotalPages = (int)Math.Ceiling((double)total / pageSize);
            this.NextPage = NextPage;
            this.PrevPage = PrevPage;
        }

        public bool ShouldSerializeNextPage()
        {
            return !string.IsNullOrWhiteSpace(NextPage);
        }

        public bool ShouldSerializePrevPage()
        {
            return !string.IsNullOrWhiteSpace(PrevPage);
        }
    }
}