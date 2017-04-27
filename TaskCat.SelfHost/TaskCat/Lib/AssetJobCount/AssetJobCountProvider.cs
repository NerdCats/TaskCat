namespace TaskCat.Lib.Catalog
{
    using Data.Entity;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using System;
    using Common.Exceptions;
    using Common.Db;
    using Common.Domain;
    using MongoDB.Bson;
    using TaskCat.Lib.AssetJobCount;
    using TaskCat.Data.Model.Identity.Response;

    public class AssetJobCountProvider : IAssetJobCountProvider
    {
        public IMongoCollection<Job> Collection { get; }

        public AssetJobCountProvider(IDbContext dbContext)
        {
            Collection = dbContext.Jobs;
        }
        
        public async Task<AssetJobCountModel> FindEligibleAssetJobCounts(string AssetID)
        {
            if (string.IsNullOrWhiteSpace(AssetID)) throw new ArgumentNullException(nameof(AssetID));

            AssetJobCountModel ajcm = new AssetJobCountModel();

            var result = await Collection.Find(x => x.Id == AssetID).FirstOrDefaultAsync();
            if (result == null)
            {
                throw new EntityNotFoundException(typeof(AssetJobCountModel), AssetID);
            }
            return ajcm;
        }
    }
}