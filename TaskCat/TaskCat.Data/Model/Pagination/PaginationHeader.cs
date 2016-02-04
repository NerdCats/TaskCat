namespace TaskCat.Data.Model.Pagination
{
    public class PaginationHeader
    {
        public long Total { get; set; }
        public int Start { get; set; }
        public int Limit { get; set; }
        public int Returned { get; set; }
    }
}