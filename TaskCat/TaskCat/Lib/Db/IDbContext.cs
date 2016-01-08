using MongoDB.Driver;

namespace TaskCat.Lib.Db
{
    public interface IDbContext
    {
        IMongoDatabase Database { get; }
    }
}