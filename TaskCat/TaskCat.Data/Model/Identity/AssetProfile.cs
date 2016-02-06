namespace TaskCat.Data.Model.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AssetProfile: UserProfile
    {
        public AssetProfile(AssetModel assetModel) : base(assetModel as UserModel)
        {
            this.NationalId = assetModel.NationalId;
            this.DriversLicenseId = assetModel.DrivingLicenceId;
        }

        public string NationalId { get; set; }
        public string DriversLicenseId { get; set; }

        // TODO: There should be something more, we need to verify an asset if necessary, if we go
        // polymorphic then we should create a new folder named Assets under Entity again and continue there. 
        // Then we can actually override an abstract property named IsVetted and start working towards it, then again,
        // if all the assets do have much in common then its pretty pointless to be honest. We should only take teh polymorphic way
        // if and only if we have really diverse properties for all the assets

    }
}
