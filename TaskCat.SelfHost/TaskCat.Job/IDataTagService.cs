namespace TaskCat.Job
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskCat.Common.Domain;
    using TaskCat.Data.Entity;

    public interface IDataTagService : IRepository<DataTag>
    {
        Task<IEnumerable<DataTag>> GetDataTagSuggestions(string q);
    }
}
