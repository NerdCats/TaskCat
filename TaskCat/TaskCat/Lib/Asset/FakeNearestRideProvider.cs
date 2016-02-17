namespace TaskCat.Lib.Asset
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity;
    using Data.Lib.Interfaces;
    using TaskCat.Data.Model;
    using Data.Model.Identity.Response;

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