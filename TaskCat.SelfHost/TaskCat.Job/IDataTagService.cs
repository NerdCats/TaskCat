namespace TaskCat.Job
{
    using System.Threading.Tasks;
    using Common.Domain;
    using Data.Entity;
    using System.Collections.Generic;
    using System;

    public interface IDataTagService : IRepository<DataTag>
    {
        [Obsolete("Use Update(string id, DataTag tag) instead")]
        new Task<DataTag> Update(DataTag tag);
        Task<DataTag> Update(string id, DataTag tag);
        Task<IEnumerable<DataTag>> GetDataTagSuggestions(string q);
    }
}
