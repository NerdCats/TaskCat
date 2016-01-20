namespace TaskCat.Lib.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model;
    using TaskCat.Data.Entity;
    using Asset;

    public interface IFetchable<T> where T : AssetEntity
    {
        Location From { get; set; }
        Location To { get; set; }
        INearestAssetProvider<T> provider { get; set; }
        Task<List<T>> FetchAvailableAssets();
        Task SelectEligibleAsset();
    }
}
