
namespace TaskCat.Lib.Asset
{
    using Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAssetProvider<T> where T : Asset
    {
        List<T> FindAssets();
    }
}
