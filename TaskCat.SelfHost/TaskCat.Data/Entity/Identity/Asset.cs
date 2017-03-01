﻿namespace TaskCat.Data.Entity.Identity
{
    using Model.Identity.Registration;
    using Model.Identity.Profile;
    using Model.Identity.Response;

    public class Asset : User
    {
        public double AverageRating { get; set; } = 0.0;
        public Asset(AssetRegistrationModel model, AssetProfile profile) : base(model, profile, RoleNames.ROLE_ASSET)
        {
        }

        public override UserModel ToModel()
        {
            return new AssetModel(this);
        }
    }
}
