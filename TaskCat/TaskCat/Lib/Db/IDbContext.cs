namespace TaskCat.Lib.Db
{
    using MongoDB.Driver;
    using Data.Entity;
    using Data.Entity.Identity;
    using Data.Entity.ShadowCat;
    using AspNet.Identity.MongoDB;

    public interface IDbContext
    {
        IMongoDatabase Database { get; }
        IMongoDatabase ShadowCatDatabase { get; }

        IMongoCollection<User> Users { get; }
        IMongoCollection<Role> Roles { get; }
        IMongoCollection<Client> Clients { get; }
        IMongoCollection<RefreshToken> RefreshTokens { get; }
        IMongoCollection<Asset> Assets { get; }
        IMongoCollection<Job> Jobs { get; }
        IMongoCollection<SupportedOrder> SupportedOrders { get; }

        IMongoCollection<AssetLocation> AssetLocations { get; }

    }
}