namespace TaskCat.Lib.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Data.Entity.Identity;
    using Microsoft.AspNet.Identity;

    public class RoleManager : RoleManager<Role>
    {
        private IRoleStore<Role, string> _store;

        public RoleManager(IRoleStore<Role, string> store)
            : base(store)
        {
            this._store = store;
        }
    }
}