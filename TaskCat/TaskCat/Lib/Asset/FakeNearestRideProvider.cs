namespace TaskCat.Lib.Asset
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Data.Model.Identity.Response;
    using TaskCat.Model.Asset;

    internal class FakeNearestRideProvider : IAssetProvider
    {
        public Task<IEnumerable<AssetWithLocationModel>> FindEligibleAssets(AssetSearchRequest assetRequest)
        {
            throw new NotImplementedException();
        }
    }
}