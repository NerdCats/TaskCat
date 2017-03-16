using TaskCat.Common.Domain;
using TaskCat.Data.Entity;

namespace TaskCat.Job
{
    public interface ITagService : IRepository<DataTag>
    {
    }
}
