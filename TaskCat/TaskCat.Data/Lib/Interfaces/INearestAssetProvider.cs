
namespace TaskCat.Data.Lib.Interfaces
{
    using Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Data.Model;

    public interface INearestAssetProvider
    {
        Task<List<Asset>> FindAssets(Location loc);
        Task<Asset> FindNearestEligibleAssets(Location from);
    }
}
