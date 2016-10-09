namespace TaskCat.Data.Model.JobTasks.Result
{
    using Geocoding;
    using Identity.Response;

    public class DefaultAssignAssetTaskResult : JobTaskResult
    {
        public DefaultAddress From { get; set; }
        public DefaultAddress To { get; set; }
        //FIXME: If asset is only person oriented we might have much
        //much simpler representation of an asset, up until that FetchRideTaskResult would be a bit complicated
        public AssetModel Asset { get; set; }

        public DefaultAssignAssetTaskResult()
        {
            this.ResultType = typeof(DefaultAssignAssetTaskResult);
        }
    }
}
