namespace TaskCat.Lib.Asset
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    using Data.Model.Identity.Response;
    using System;

    internal class FakeNearestRideProvider : IAssetProvider
    {

        public Task<List<AssetModel>> FindNearestAssets(Location location, double radius)
        {
            throw new NotImplementedException();
        }

        public Task<List<AssetModel>> FindNearestAssets(Location location, double radius, int limit = 10)
        {
            throw new NotImplementedException();
        }

        public Task<AssetModel> FindNearestEligibleAssets(Location location)
        {
            throw new NotImplementedException();
        }
    }
}