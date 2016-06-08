namespace TaskCat.Lib.DropPoint
{
    using Domain;
    using Data.Entity;
    using MongoDB.Driver;

    public interface IDropPointService: IRepository<DropPoint>
    {
        IMongoCollection<DropPoint> Collection { get; set; }
    }
}