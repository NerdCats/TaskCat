namespace TaskCat.Lib.Auth
{
    using AspNet.Identity.MongoDB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Entity.Identity;
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using Data.Model.Identity;
    using Data.Entity;

    public class AccountStore : UserStore<User>
    {
        IMongoCollection<User> collection;
        public AccountStore(IMongoCollection<User> users) : base(users)
        {
            collection = users;
        }

        public async Task<List<User>> FindAll(int start, int limit)
        {
            return await collection.Find(x => true).Skip(start).Limit(limit).ToListAsync();
        }

        public async Task<List<T>> FindAll<T>() where T : User
        {
            Type type = typeof(T);
            if (type == typeof(User))
                return await collection.Find(x => x.Type == IdentityTypes.FETCHER).ToListAsync() as List<T>;
            else if (type == typeof(Asset))
                return await collection.Find(x => x.Type != IdentityTypes.FETCHER).ToListAsync() as List<T>;

            throw new InvalidOperationException("Identity Type not supported yet");
        }

        public async Task<T> FindAsByAsync<T>(string username, string passwordHash) where T : User
        {
            Type type = typeof(T);

            if (type == typeof(User))
                return await collection.Find(x => (x.Type == IdentityTypes.FETCHER && x.PasswordHash == passwordHash && x.UserName == username)).FirstOrDefaultAsync() as T;

            else if (type == typeof(Asset))
                return await collection.Find(x => (x.Type != IdentityTypes.FETCHER && x.PasswordHash == passwordHash && x.UserName == username)).FirstOrDefaultAsync() as T;

            throw new InvalidOperationException("Identity Type not supported yet");
        }

        internal async Task<long> GetUserCountAsync()
        {
            return await collection.CountAsync(Builders<User>.Filter.Empty);
        }
    }
}