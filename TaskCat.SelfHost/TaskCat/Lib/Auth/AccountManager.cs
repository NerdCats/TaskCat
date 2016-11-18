namespace TaskCat.Lib.Auth
{
    using Microsoft.AspNet.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Entity.Identity;
    using Data.Model.Identity;
    using Data.Model.Identity.Response;
    using MongoDB.Driver;
    using Exceptions;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security.DataProtection;
    using Common.Exceptions;

    public class AccountManager : UserManager<User>
    {
        AccountStore accountStore;

        public AccountManager(IUserStore<User> store) : base(store)
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

        public AccountManager(IUserStore<User> store, IDataProtectionProvider dataProtectionProvider) : this(store)
        {
            //TODO: Define proper user token protection provider token here

            this.UserTokenProvider = new DataProtectorTokenProvider<User, string>(dataProtectionProvider.Create("Email Notification"))
            {
                TokenLifespan = TimeSpan.FromHours(6)
            };
        }

        public async Task<User> FindByEmailAsync(string email, string password)
        {
            return await FindByEmailAsync<User>(email, password);
        }

        public async Task<T> FindByEmailAsync<T>(string email, string password) where T : User
        {
            var user = await FindByEmailAsync(email);
            if (user == null) return null;

            var verify = PasswordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (verify.ToString() == "Success")
            {
                Type type = typeof(T);

                if ((type == typeof(User) && user.Type == IdentityTypes.USER)
                    || (type == typeof(Asset) && user.Type != IdentityTypes.USER))
                    return user as T;
                return null;
            }
            else
            {
                return null;
            }
        }

        public async Task<User> FindByUserNameOrEmailOrPhoneNumber(string userKey)
        {
            var user = await accountStore.FindUserByUserNameOrPhoneNumberOrEmail(userKey);
            return user;
        }

        public async Task<User> FindByPhoneNumber(string phoneNumber)
        {
            var user = await accountStore.FindUserByPhoneNumber(phoneNumber);
            return user;
        }

        public override async Task<User> FindByIdAsync(string userId)
        {
            var user = await base.FindByIdAsync(userId);
            if (user == null)
                throw new EntityNotFoundException("User", userId);
            return user;
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

        internal async Task<IQueryable<UserModel>> FindAllAsModel()
        {
            return await accountStore.FindAllAsModel();
        }

        internal async Task<IQueryable<UserModel>> FindAllAsModelAsQueryable(int start, int limit)
        {
            return await accountStore.FindAllAsModelAsQueryable(start, limit);
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