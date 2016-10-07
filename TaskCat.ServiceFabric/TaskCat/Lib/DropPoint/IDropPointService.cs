namespace TaskCat.Lib.DropPoint
{
    using Domain;
    using Data.Entity;
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IDropPointService: IRepository<DropPoint>
    { 
        Task<IEnumerable<DropPoint>> SearchDropPoints(string userId, string query);
        Task<DropPoint> Update(DropPoint value, string userId);
        Task<DropPoint> Delete(string id, string userId);
        Task<DropPoint> Get(string id, string userId);
    }
}