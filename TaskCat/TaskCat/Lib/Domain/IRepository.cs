namespace TaskCat.Lib.Domain
{
    using MongoDB.Driver;
    using System.Threading.Tasks;

    public interface IRepository<T>
    {
        Task<T> Insert(T obj);
        Task<T> Delete(string id);
        Task<T> Get(string id);
        Task<T> Update(T obj);
    }
}