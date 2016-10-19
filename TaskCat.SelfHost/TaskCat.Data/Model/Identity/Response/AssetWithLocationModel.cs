namespace TaskCat.Data.Model.Identity.Response
{
    using Entity.ShadowCat;
    using Entity.Identity;

    public class AssetWithLocationModel : AssetModel
    {
        public AssetLocation Location;

        public AssetWithLocationModel(Asset asset, AssetLocation location, bool isUserAuthenticated = false) :
            base(asset, isUserAuthenticated)
        {
            this.Location = location;
            Location.IgnoreAssetId = true;
        }
    }
}
