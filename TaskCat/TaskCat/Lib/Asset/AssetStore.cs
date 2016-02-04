using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TaskCat.Data.Entity;
using TaskCat.Lib.Db;

namespace TaskCat.Lib.Asset
{
    public class AssetStore
    {
        private IDbContext dbContext;
        public AssetStore(IDbContext context)
        {
            this.dbContext = context;
        }
        internal async Task CreateOneAsync(AssetEntity assetEntity)
        {
            await this.dbContext.Assets.InsertOneAsync(assetEntity);
        }
    }
}