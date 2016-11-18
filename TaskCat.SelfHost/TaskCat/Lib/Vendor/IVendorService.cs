namespace TaskCat.Lib.Vendor
{
    using Data.Entity;
    using Common.Domain;
    using System.Threading.Tasks;

    public interface IVendorService : IRepository<Vendor>
    {
        Task<SubscriptionResult> Subscribe(Vendor vendor);
    }
}