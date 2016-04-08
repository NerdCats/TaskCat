namespace TaskCat.Lib.Asset
{
    using Data.Model;
    using Data.Model.Identity.Response;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// INearestAssetProvider is the default interface for 
    /// providing needed assets in time
    /// </summary>
    public interface INearestAssetProvider
    {
        /// <summary>
        /// Find all Assets around a location
        /// </summary>
        /// <param name="location">
        /// Location you need assets around
        /// </param>
        /// <param name="radius">
        /// radius in meters to do the available asset search
        /// </param>
        /// <returns>
        /// List of AssetModels around that location
        /// </returns>
        /// 
        Task<List<AssetModel>> FindNearestAssets(Location location, double radius, int limit = 10);
        /// <summary>
        /// Find all nearest eligible assets around a location
        /// </summary>
        /// <param name="location">
        /// Location you need assets around
        /// </param>
        /// <returns>
        /// List of eligible AssetModels around a location
        /// </returns>
        Task<AssetModel> FindNearestEligibleAssets(Location location);
    }
}
