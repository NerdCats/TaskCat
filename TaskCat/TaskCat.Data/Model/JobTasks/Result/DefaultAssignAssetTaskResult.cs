namespace TaskCat.Data.Model.JobTasks.Result
{
    using Identity.Response;

    public class DefaultAssignAssetTaskResult : JobTaskResult
    {
        public Location From { get; set; }
        public Location To { get; set; }
        //FIXME: If asset is only person oriented we might have much
        //much simpler representation of an asset, up until that FetchRideTaskResult would be a bit complicated
        public AssetModel Asset { get; set; }

        public DefaultAssignAssetTaskResult()
        {
            this.ResultType = typeof(DefaultAssignAssetTaskResult);
        }
    }
}
