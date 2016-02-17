namespace TaskCat.Data.Lib.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    using TaskCat.Data.Entity;
    using Model.Identity.Response;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IFetchable
    {
        Location From { get; set; }
        Location To { get; set; }
        INearestAssetProvider provider { get; set; }
        Task<List<AssetModel>> FetchAvailableAssets();
        Task SelectEligibleAsset();
    }
}
