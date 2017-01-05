namespace TaskCat.Lib.AssetProvider
{
    using Account.Core;
    using Asset;
    using Common.Db;
    using Data.Entity.ShadowCat;
    using Data.Model.Identity.Response;
    using Data.Model.ShadowCat;
    using Db;
    using Model.Asset;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Default implementation of asset provider, essentially provides assets
    /// for a job
    /// </summary>
    internal class DefaultAssetProvider : IAssetProvider
    {
        private readonly AccountManager _accountManager;
        private readonly IDbContext _dbContext;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DefaultAssetProvider(AccountManager accountManager, IDbContext dbContext)
        {
            this._accountManager = accountManager;
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<AssetWithLocationModel>> FindEligibleAssets(AssetSearchRequest assetRequest)
        {
            if (assetRequest.Location.Point == null)
            {
                logger.Error("assetRequest.Location.Point is null");
                throw new NotImplementedException("Blank point with address is not supported yet");
            }
            if (assetRequest.Strategy == SearchStrategy.DEEP)
            {
                logger.Error("Deep search is not implemented yet");
                throw new NotImplementedException("Deep search is not implemented yet");
            }

            var nearestAssets = await GetNearestAssetLocations(assetRequest);
            var result = await nearestAssets.Populate(x => x.Asset_Id, _dbContext.Assets);

            return result.Select(x => new AssetWithLocationModel(x.TDoc, x.FDoc));
        }

        private async Task<IEnumerable<AssetLocation>> GetNearestAssetLocations(AssetSearchRequest assetRequest)
        {
            var geoNearOptions = new BsonDocument {
                { "near", new BsonDocument {
                    { "type", "Point" },
                    { "coordinates", new BsonArray(assetRequest.Location.Point.coordinates) },
                } },
                { "distanceField", "Distance" },
                { "maxDistance", assetRequest.Radius },
                { "limit" , assetRequest.Limit },
                { "spherical" , true }
            };

            var pipeline = new List<BsonDocument>();
            pipeline.Add(new BsonDocument { { "$geoNear", geoNearOptions } });

            var result = new List<AssetLocationWithDistance>();

            using (var cursor = await _dbContext.AssetLocations.AggregateAsync<BsonDocument>(pipeline))
            {
                while (await cursor.MoveNextAsync())
                {
                    result.AddRange(cursor.Current.Select(x => BsonSerializer.Deserialize<AssetLocationWithDistance>(x)));
                }
            }

            return result;
        }
    }
}