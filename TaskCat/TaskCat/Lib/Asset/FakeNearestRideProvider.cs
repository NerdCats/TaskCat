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

    internal class FakeNearestRideProvider : INearestAssetProvider
    {
        public Task<List<Data.Entity.Asset>> FindAssets(Location loc)
        {
            return null;
        }

        public async Task<Data.Entity.Asset> FindNearestEligibleAssets(Location from)
        {
            return null;
        }
    }
}