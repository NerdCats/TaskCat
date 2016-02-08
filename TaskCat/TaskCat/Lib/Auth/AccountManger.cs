namespace TaskCat.Lib.Auth
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using TaskCat.Data.Entity.Identity;
    using TaskCat.Data.Entity;
    using Data.Model.Identity;

    public class AccountManger : UserManager<User>
    {
        AccountStore accountStore;
        public AccountManger(IUserStore<User> store) : base(store)
        {
            accountStore = store as AccountStore;
        }

        public async Task<T> FindAsByIdAsync<T>(string id) where T : User
        {
            return await Store.FindByIdAsync(id) as T;
        }

        public async Task<T> FindAsByAsync<T>(string username, string password) where T : User
        {
            return await accountStore.FindAsByAsync<T>(username, PasswordHasher.HashPassword(password));
        }

        public async Task<List<T>> FindAll<T>() where T : User
        {
            return await accountStore.FindAll<T>();
        }

        public async Task<List<User>> FindAll(int start, int limit)
        {
            return await accountStore.FindAll(start, limit);
        }

        internal async Task<long> GetTotalUserCount()
        {
            return await accountStore.GetUserCountAsync();
        }
    }
}