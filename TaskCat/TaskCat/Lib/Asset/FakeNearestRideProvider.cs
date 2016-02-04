using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskCat.Data.Entity;
using TaskCat.Data.Model;

namespace TaskCat.Lib.Asset
{
    public class FakeNearestRideProvider : INearestAssetProvider<AssetEntity>
    {
        public Task<List<AssetEntity>> FindAssets(Location loc)
        {
            return null;
        }

        public async Task<AssetEntity> FindNearestEligibleAssets(Location from)
        {
            return null;
        }
    }
}