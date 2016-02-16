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
    using Data.Model.Identity.Response;
    using MongoDB.Driver;

    public class AccountManger : UserManager<User>
    {
        AccountStore accountStore;
        public AccountManger(IUserStore<User> store) : base(store)
        {
            accountStore = store as AccountStore;
            UserValidator = new UserValidator<User>(this)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6
            };
            
        }

        public async Task<User> FindByEmailAsync(string email, string password)
        {
            var user = await FindByEmailAsync(email);
            if (user == null) return user;
            
            var verify = PasswordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (verify.ToString()=="Success")
                return user;
            else return null;
        }

        public async Task<T> FindAsByIdAsync<T>(string id) where T : User
        {
            var user = await Store.FindByIdAsync(id);

            Type type = typeof(T);

            if ((type == typeof(User) && user.Type == IdentityTypes.USER) 
                || (type == typeof(Asset) && user.Type != IdentityTypes.USER))
                return user as T;
            return null;
        }

        public async Task<List<T>> FindAll<T>() where T : User
        {
            return await accountStore.FindAll<T>();
        }

        public async Task<List<User>> FindAll(int start, int limit)
        {
            return await accountStore.FindAll(start, limit);
        }

        public async Task<List<UserModel>> FindAllAsModel(int start, int limit)
        {
            return await accountStore.FindAllAsModel(start, limit);
        }

        internal async Task<IEnumerable<UserModel>> FindAllAsModel()
        {
            return await accountStore.FindAllAsModel();
        }

        public async Task<UpdateResult> ChangeAvatar(string userId, string avatarUrl)
        {
            return await accountStore.SetAvatarAsync(userId, avatarUrl);
        }

        internal async Task<long> GetTotalUserCount()
        {
            return await accountStore.GetUserCountAsync();
        }

        
    }
}