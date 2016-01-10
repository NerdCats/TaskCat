using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskCat.Data.Entity.Assets;
using TaskCat.Data.Model;

namespace TaskCat.Lib.Asset
{
    public class FakeNearestRideProvider : INearestAssetProvider<Ryde>
    {
        public async Task<List<Ryde>> FindAssets(Location loc)
        {
            return null;
        }
    }
}