namespace TaskCat.Lib.Catalog
{
    using Data.Entity;
    using Domain;
    using MongoDB.Driver;

    public interface IStoreService : IRepository<Store>
    {
        IMongoCollection<Store> Collection { get;}
    }
}
