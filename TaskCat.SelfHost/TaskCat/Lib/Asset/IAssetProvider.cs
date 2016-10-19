namespace TaskCat.Lib.Asset
{
    using Data.Model.Identity.Response;
    using Model.Asset;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// INearestAssetProvider is the default interface for 
    /// providing needed assets in time
    /// </summary>
    public interface IAssetProvider
    {
        /// <summary>
        /// Finds Eligible Assets based on an Asset Request
        /// </summary>
        /// <param name="assetRequest"></param>
        /// <returns></returns>
        Task<IEnumerable<AssetWithLocationModel>> FindEligibleAssets(AssetSearchRequest assetRequest);
    }
}
