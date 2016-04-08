namespace TaskCat.Lib.AssetProvider
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Model;
    using Data.Model.Identity.Response;
    using Auth;
    using Asset;

    /// <summary>
    /// Default implementation of asset provider, essentially provides assets
    /// for a job
    /// </summary>

    public class DefaultAssetProvider : IAssetProvider
    {
        private readonly AccountManger accountManager;
        public DefaultAssetProvider()
        {

        }

        public Task<List<AssetModel>> FindNearestAssets(Location location, double radius)
        {
            //

            throw new NotImplementedException();
        }

        public Task<AssetModel> FindNearestEligibleAssets(Location location)
        {
            throw new NotImplementedException();
        }
    }
}