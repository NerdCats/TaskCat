namespace TaskCat.Lib.Db
{
    using MongoDB.Driver;
    using TaskCat.Data.Entity;
    using Data.Entity.Identity;

    public interface IDbContext
    {
        IMongoDatabase Database { get; }

        IMongoCollection<User> Users { get; }
        IMongoCollection<Role> Roles { get; }
        IMongoCollection<Client> Clients { get; }
        IMongoCollection<RefreshToken> RefreshTokens { get; }

        IMongoCollection<JobEntity> Jobs { get; }
        
    }
}