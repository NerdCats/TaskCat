namespace TaskCat.Data.Model.Query
{
    using Entity;
    using System.Collections.Generic;

    public class QueryResult<T> where T : DbEntity
    {
        public long Total { get; set; }
        public IEnumerable<T> Result { get; set; }

        public QueryResult()
        {

        }

        public QueryResult(IEnumerable<T> result, long total)
        {
            this.Total = total;
            this.Result = result;
        }
    }
}
