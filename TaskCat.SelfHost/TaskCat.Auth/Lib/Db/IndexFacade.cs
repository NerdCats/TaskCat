namespace TaskCat.Auth.Lib.Db
{
    using MongoDB.Driver;
    using TaskCat.Data.Entity.Identity;

    public class IndexFacade
    {
        public static void EnsureUniqueIndexOnPhoneNumber(IMongoCollection<User> userCollection)
        {
            CreateIndexOptions<User> options = new CreateIndexOptions<User>();
            options.Unique = true;
            options.Sparse = true;

            userCollection.Indexes.CreateOne(Builders<User>.IndexKeys.Ascending(x => x.PhoneNumber), options);
            userCollection.Indexes.CreateOne(Builders<User>.IndexKeys.Descending(x => x.PhoneNumber), options);
        }

        public static void EnsureIndexesOnUserType(IMongoCollection<User> userCollection)
        {
            CreateIndexOptions<User> options = new CreateIndexOptions<User>();
            options.Background = true;

            userCollection.Indexes.CreateOne(Builders<User>.IndexKeys.Ascending(x => x.Type), options);
            userCollection.Indexes.CreateOne(Builders<User>.IndexKeys.Descending(x => x.Type), options);
        }
    }
}
