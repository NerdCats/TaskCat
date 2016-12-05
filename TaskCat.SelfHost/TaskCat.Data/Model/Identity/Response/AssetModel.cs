namespace TaskCat.Data.Model.Identity.Response
{
    using Entity.Identity;
    using Profile;

    public class AssetModel : UserModel
    {
        public double AverageRating { get; set; }

        public AssetModel()
        {

        }

        public AssetModel(Asset asset, bool isUserAuthenticated = false) : base(asset, isUserAuthenticated)
        {
            this.Profile = asset.Profile as AssetProfile;
            this.AverageRating = asset.AverageRating;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty((this.Profile as AssetProfile).FullName)
                ? this.UserName : (this.Profile as AssetProfile).FullName;
        }
    }
}
