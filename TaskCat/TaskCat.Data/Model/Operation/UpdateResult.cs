namespace TaskCat.Data.Model.Operation
{
    using Entity;

    public class UpdateResult<T> where T: DbEntity
    {
        public long MatchedCount { get; set; }
        public long ModifiedCount { get; set; }
        public T UpdatedValue { get; set; }

        public UpdateResult(long matchedCount, long modifiedCount, T updatedValue)
        {
            MatchedCount = matchedCount;
            ModifiedCount = modifiedCount;
            UpdatedValue = updatedValue;
        }
    }
}
