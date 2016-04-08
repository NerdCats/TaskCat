namespace TaskCat.Lib.AssetProvider
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Model.Identity.Response;
    using Auth;
    using Asset;
    using Model.Asset;
    using Db;
    using System;
    using MongoDB.Driver.GeoJsonObjectModel;
    using MongoDB.Driver;
    using Data.Entity.ShadowCat;
    using System.Linq;

    /// <summary>
    /// Default implementation of asset provider, essentially provides assets
    /// for a job
    /// </summary>
    internal class DefaultAssetProvider : IAssetProvider
    {
        private readonly AccountManger _accountManager;
        private readonly IDbContext _dbContext;

        public DefaultAssetProvider(AccountManger accountManager, IDbContext dbContext)
        {
            this._accountManager = accountManager;
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<AssetWithLocationModel>> FindEligibleAssets(AssetSearchRequest assetRequest)
        {
            if (assetRequest.Location.Point == null)
                throw new NotImplementedException("Blank point with address is not supported yet");
            if (assetRequest.Strategy == SearchStrategy.DEEP)
                throw new NotImplementedException("Deep search is not implemented yet");

            var nearestAssets = await GetNearestAssetLocations(assetRequest);
            var result = await nearestAssets.Populate(x => x.Asset_Id, _dbContext.Assets);

            return result.Select(x => new AssetWithLocationModel(x.TDoc, x.FDoc));
        }

        private async Task<List<AssetLocation>> GetNearestAssetLocations(AssetSearchRequest assetRequest)
        {
            GeoJson2DGeographicCoordinates geoLocation = new GeoJson2DGeographicCoordinates(assetRequest.Location.Point.coordinates[0], assetRequest.Location.Point.coordinates[1]);
            GeoJsonPoint<GeoJson2DGeographicCoordinates> geoPoint = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(geoLocation);

            FilterDefinitionBuilder<AssetLocation> builder = new FilterDefinitionBuilder<AssetLocation>();
            FilterDefinition<AssetLocation> Filter = builder.NearSphere(x => x.Point, geoPoint, assetRequest.Radius);

            return await _dbContext.AssetLocations.Find(Filter).ToListAsync();
        }
    }
}