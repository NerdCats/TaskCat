namespace TaskCat.Lib.Vendor
{
    using System.Threading.Tasks;

    public interface IVendorService
    {
        Task<bool> Subscribe(string userId);
    }
}