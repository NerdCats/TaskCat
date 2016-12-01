namespace TaskCat.Data.Model.Operation
{
    using Entity;
    using Newtonsoft.Json;

    public class UpdateResult<T> where T: DbEntity
    {
        [JsonIgnore]
        public bool SerializeUpdatedValue { get; set; }
        public long MatchedCount { get; set; }
        public long ModifiedCount { get; set; }
        public T UpdatedValue { get; set; }

        public UpdateResult(long matchedCount, long modifiedCount, T updatedValue)
        {
            MatchedCount = matchedCount;
            ModifiedCount = modifiedCount;
            UpdatedValue = updatedValue;
        }

        public bool ShouldSerializeUpdatedValue()
        {
            return SerializeUpdatedValue;
        }
    }
}
