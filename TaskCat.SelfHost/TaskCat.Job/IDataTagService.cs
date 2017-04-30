namespace TaskCat.Job
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Domain;
    using Data.Entity;

    public interface IDataTagService : IRepository<DataTag>
    {
        Task<IEnumerable<DataTag>> GetDataTagSuggestions(string q);
    }
}
