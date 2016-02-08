namespace TaskCat.Data.Model.Identity.Response
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
