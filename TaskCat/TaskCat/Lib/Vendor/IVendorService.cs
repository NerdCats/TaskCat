namespace TaskCat.Lib.Vendor
{
    using System.Threading.Tasks;

    public interface IVendorService
    {
        Task<SubscriptionResult> Subscribe(string userId);
    }
}