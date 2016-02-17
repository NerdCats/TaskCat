
namespace TaskCat.Data.Lib.Interfaces
{
    using Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Data.Model;
    using Model.Identity.Response;
    public interface INearestAssetProvider
    {
        Task<List<AssetModel>> FindAssets(Location loc);
        Task<AssetModel> FindNearestEligibleAssets(Location from);
    }
}
