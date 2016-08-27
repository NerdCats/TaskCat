namespace TaskCat.Lib.Vendor
{
    using Data.Entity;
    using Domain;
    using System.Threading.Tasks;

    public interface IVendorService : IRepository<VendorProfile>
    {
        Task<SubscriptionResult> Subscribe(string userId, VendorProfile profile);
    }
}