namespace TaskCat.Lib.Auth
{
    using AspNet.Identity.MongoDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.Entity.Identity;
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using Data.Model.Identity;
    using Data.Model.Identity.Response;
    using Microsoft.AspNet.Identity;
    using Utility;
    using Exceptions;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AccountStore : UserStore<User>
    {
        IMongoCollection<User> collection;
        public AccountStore(IMongoCollection<User> users) : base(users)
        {
            collection = users;
        }

        public async Task<User> FindUserByUserNameOrPhoneNumberOrEmail(string userKey)
        {
            var user = await collection.Find(x => x.UserName == userKey || x.PhoneNumber == userKey || x.Email == x.Email).FirstOrDefaultAsync();
            if (user == null)
                throw new EntityNotFoundException("User", userKey);
            return user;
        }

        public async Task<List<User>> FindAll(int start, int limit)
        {
            return await collection.Find(x => true).Skip(start).Limit(limit).ToListAsync();
        }

        internal async Task<IQueryable<UserModel>> FindAllAsModel()
        {
            return await Task.Run(() =>
            {
                return collection.Find(x => true).ToEnumerable().Select(x =>
                {
                    return x.ToModel(true);
                }).AsQueryable();
            });

        }

        public async Task<List<UserModel>> FindAllAsModel(int start, int limit)
        {
            // INFO: For all the smart people who'd tell me to go for an IEnumerable here, 
            // I would, definitely would when Id go for OData but in this case I have a pageSize limit
            // And Id like to keep my performance over lines of code.
            // Lining Select over Linq over an IEnumerable wasnt breathing well in benchmark

            List<UserModel> returnList = new List<UserModel>();
            using (var cursor = await collection.Find(x => true).Skip(start).Limit(limit).ToCursorAsync())
            {
                await cursor.ForEachAsync(x =>
                {
                    var model = x.ToModel(true);
                    returnList.Add(model);
                });
            }

            return returnList;
        }

        internal async Task<IQueryable<UserModel>> FindAllAsModelAsQueryable(int start, int limit)
        {
            return await Task.Run(() =>
            {
                return collection.Find(x => true).Skip(start).Limit(limit).ToEnumerable().Select(x =>
                {
                    return x.ToModel(true);
                }).AsQueryable();
            });
        }



        public async Task<List<T>> FindAll<T>() where T : User
        {
            Type type = typeof(T);
            if (type == typeof(User))
                return await collection.Find(x => x.Type == IdentityTypes.USER).ToListAsync() as List<T>;
            else if (type == typeof(Asset))
                return await collection.Find(x => x.Type != IdentityTypes.USER && x.Type != IdentityTypes.ENTERPRISE).ToListAsync() as List<T>;
            else if (type == typeof(EnterpriseUser))
                return await collection.Find(x => x.Type == IdentityTypes.ENTERPRISE).ToListAsync() as List<T>;

            throw new InvalidOperationException("Identity Type not supported yet");
        }



        internal async Task<long> GetUserCountAsync()
        {
            return await collection.CountAsync(Builders<User>.Filter.Empty);
        }

        internal async Task<UpdateResult> SetAvatarAsync(string userId, string picUrl)
        {
            UpdateDefinition<User> updateDefinition = Builders<User>.Update.Set(x => x.Profile.PicUri, picUrl);
            var result = await collection.UpdateOneAsync(Builders<User>.Filter.Where(x => x.Id == userId), updateDefinition);
            return result;
        }


    }
}