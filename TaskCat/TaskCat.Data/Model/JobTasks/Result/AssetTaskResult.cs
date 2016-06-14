namespace TaskCat.Data.Model.JobTasks.Result
{
    using Identity.Response;

    public class AssetTaskResult : JobTaskResult
    {
        public AssetModel Asset { get; set; }
        public AssetTaskResult()
        {
            this.ResultType = typeof(AssetTaskResult);
        }
    }
}
