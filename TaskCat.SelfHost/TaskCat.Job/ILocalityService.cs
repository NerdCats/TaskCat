namespace TaskCat.Job
{
    using Data.Entity;
    using MongoDB.Driver;
    using System.Threading.Tasks;

    public interface ILocalityService
    {
        IMongoCollection<Locality> Collection { get; }
        Task RefreshLocalities();
    }
}
