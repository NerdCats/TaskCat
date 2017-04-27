namespace TaskCat.Lib.AssetJobCount
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Data.Model.Identity.Response;

    /// <summary>
    /// INearestAssetJobCountProvider is the default interface for 
    /// providing needed AssetJobCounts in time
    /// </summary>
    public interface IAssetJobCountProvider
    {
        /// <summary>
        /// Finds Eligible AssetJobCounts based on an AssetID Request
        /// </summary>
        /// <param name="AssetID"></param>
        /// <returns></returns>
        Task<AssetJobCountModel> FindEligibleAssetJobCounts(string AssetID);
    }
}
