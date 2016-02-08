namespace TaskCat.Data.Model.Pagination
{
    public class PaginationHeader
    {
        public long Total { get; set; }
        public long Start { get; set; }
        public int PageSize { get; set; }
        public int Returned { get; set; }
        public string NextPage { get; set; }
        public string PrevPage { get; set; }

        public PaginationHeader(long total, long start, int pageSize, int returned)
        {
            this.Total = total;
            this.Start = start;
            this.PageSize = pageSize;
            this.Returned = returned;
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