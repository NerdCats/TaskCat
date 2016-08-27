namespace TaskCat.Lib.Db
{
    using MongoDB.Driver;
    using Data.Entity;
    using Data.Entity.Identity;
    using Data.Entity.ShadowCat;
    using System;

    public interface IDbContext : IDisposable
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
        IMongoCollection<HRIDEntity> HRIDs { get; }
        IMongoCollection<DropPoint> DropPoints { get; }
        IMongoCollection<Store> Stores { get; }
        IMongoCollection<ProductCategory> ProductCategories { get; }
        IMongoCollection<VendorProfile> VendorProfiles { get; }
    }
}