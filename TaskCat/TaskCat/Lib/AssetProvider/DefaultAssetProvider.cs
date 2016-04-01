namespace TaskCat.Lib.AssetProvider
{
    using Data.Lib.AssetProvider;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Threading.Tasks;
    using Data.Model;
    using Data.Model.Identity.Response;

    /// <summary>
    /// Default implementation of asset provider, essentially provides assets
    /// for a job
    /// </summary>

    public class DefaultAssetProvider : INearestAssetProvider
    {
        public Task<List<AssetModel>> FindAssets(Location location)
        {
            throw new NotImplementedException();
        }

        public Task<AssetModel> FindNearestEligibleAssets(Location location)
        {
            throw new NotImplementedException();
        }
    }
}