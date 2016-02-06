using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskCat.Data.Entity;
using TaskCat.Data.Model;

namespace TaskCat.Lib.Asset
{
    public class FakeNearestRideProvider : INearestAssetProvider<Data.Entity.Asset>
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