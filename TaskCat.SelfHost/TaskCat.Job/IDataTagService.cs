namespace TaskCat.Job
{
    using System.Threading.Tasks;
    using Common.Domain;
    using Data.Entity;
    using System.Collections.Generic;

    public interface IDataTagService : IRepository<DataTag>
    { 
        Task<IEnumerable<DataTag>> GetDataTagSuggestions(string q);
    }
}
