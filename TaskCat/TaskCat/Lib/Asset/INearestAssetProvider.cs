
namespace TaskCat.Lib.Asset
{
    using Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Data.Model;

    public interface INearestAssetProvider<T> where T : Asset
    {
        Task<List<T>> FindAssets(Location loc);
        Task<T> FindNearestEligibleAssets(Location from);
    }
}
