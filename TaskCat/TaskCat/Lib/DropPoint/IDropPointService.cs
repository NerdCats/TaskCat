namespace TaskCat.Lib.DropPoint
{
    using Domain;
    using Data.Entity;
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IDropPointService: IRepository<DropPoint>
    {
        IMongoCollection<DropPoint> Collection { get; set; }

        Task<IEnumerable<DropPoint>> SearchDropPoints(string userId, string query);
    }
}