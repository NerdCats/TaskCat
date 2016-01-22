namespace TaskCat.Lib.Db
{
    using MongoDB.Driver;
    using TaskCat.Data.Entity;

    public interface IDbContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<JobEntity> Jobs { get; }
    }
}