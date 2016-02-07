namespace TaskCat.Lib.Asset
{
    using TaskCat.Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Auth;
    using Db;

    public class AssetManager : UserManager
    {
    
        public AssetManager(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}