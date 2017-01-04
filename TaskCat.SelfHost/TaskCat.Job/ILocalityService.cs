namespace TaskCat.Job
{
    using System.Threading.Tasks;
    public interface ILocalityService
    {
        Task RefreshLocalities();
    }
}
