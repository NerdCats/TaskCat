namespace TaskCat.Lib.Asset
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    using Data.Model.Identity.Response;
    using Data.Lib.Asset;

    internal class FakeNearestRideProvider : INearestAssetProvider
    {
        public Task<List<AssetModel>> FindAssets(Location loc)
        {
            return null;
        }

        public async Task<AssetModel> FindNearestEligibleAssets(Location from)
        {
            return null;
        }
    }
}