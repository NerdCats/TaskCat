namespace TaskCat.Data.Model.Identity.Response
{
    using Entity;

    public class AssetModel : UserModel
    {
        public double AverageRating { get; set; }

        public AssetModel()
        {

        }

        public AssetModel(Asset asset, bool isUserAuthenticated = false) : base(asset, isUserAuthenticated)
        {
            this.AverageRating = asset.AverageRating;
        }
    }
}
