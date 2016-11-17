namespace TaskCat.Auth.Lib.Db
{
    using Data.Entity.Identity;
    using MongoDB.Driver;

    public class IDbContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<User> Users { get; }
        IMongoCollection<Role> Roles { get; }
        IMongoCollection<Client> Clients { get; }
        IMongoCollection<RefreshToken> RefreshTokens { get; }
    }
}
