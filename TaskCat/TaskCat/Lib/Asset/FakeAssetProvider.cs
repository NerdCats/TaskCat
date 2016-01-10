using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskCat.Data.Entity.Assets;

namespace TaskCat.Lib.Asset
{
    public class FakeRydeProvider : IAssetProvider<Ryde>
    {
        public List<Ryde> FindAssets()
        {
            throw new NotImplementedException();
        }
    }
}