namespace TaskCat.Data.Lib.Asset
{
    using Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Data.Model;
    using Model.Identity.Response;

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
        /// <returns>
        /// List of AssetModels around that location
        /// </returns>
        Task<List<AssetModel>> FindAssets(Location location);
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
