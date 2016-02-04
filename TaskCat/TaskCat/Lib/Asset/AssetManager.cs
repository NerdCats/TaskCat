using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskCat.Data.Entity;
using TaskCat.Lib.Db;
using System.Threading.Tasks;

namespace TaskCat.Lib.Asset
{
    public class AssetManager
    {   
        private AssetStore _store;
        public AssetManager(AssetStore store)
        {
            this._store = store;
        }

        internal async Task<AssetEntity> CreateAsync(AssetEntity assetEntity)
        {
            await _store.CreateOneAsync(assetEntity);
            return assetEntity;
        }
    }
}