using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        internal async Task<AssetEntity> FindOneByUserIdAsync(string userId)
        {
            return await this.dbContext.Assets.Find(x => x.UserRef == userId).FirstAsync();
        }

        internal async Task<AssetEntity> FindOneAsync(string id)
        {
            return await this.dbContext.Assets.Find(x => x.id == id).FirstAsync();
        }

        //INFO: Just exposing this just so manager becomes more capable of building more queries
        internal async Task<List<AssetEntity>> Find(Expression<Func<AssetEntity, bool>> filter)
        {
            return await this.dbContext.Assets.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Updates an Asset
        /// </summary>
        /// <param name="asset">
        /// The replacement Asset that needs to be updated
        /// </param>
        /// <param name="filter">
        /// Filter that finds that asset, if filter is null, asset id is used to match asset
        /// </param>
        /// <param name="isUpsert">
        /// isUpsert is by default false, if provided true and it fails to find the asset by the filter given here, inserts a new one
        /// </param>
        /// <returns></returns>
        internal async Task<ReplaceOneResult> UpdateOneAsync(AssetEntity asset, Expression<Func<AssetEntity, bool>> filter=null, bool isUpsert=false)
        {
            UpdateOptions options = new UpdateOptions();
            options.IsUpsert = isUpsert;

            if (filter == null)
                filter = x => x.id == asset.id;

            return await this.dbContext.Assets.ReplaceOneAsync(filter, asset, options);
        }

    }
}