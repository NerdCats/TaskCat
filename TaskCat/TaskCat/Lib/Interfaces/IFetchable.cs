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
        Location FromLocation { get; set; }
        Location ToLocation { get; set; }
        Task<List<T>> FetchAvailableAssets(INearestAssetProvider<T> provider);
    }
}
