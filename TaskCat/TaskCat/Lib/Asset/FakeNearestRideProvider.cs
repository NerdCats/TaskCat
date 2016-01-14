using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskCat.Data.Entity.Assets;
using TaskCat.Data.Model;

namespace TaskCat.Lib.Asset
{
    public class FakeNearestRideProvider : INearestAssetProvider<Ride>
    {
        public async Task<List<Ride>> FindAssets(Location loc)
        {
            return null;
        }
    }
}