namespace TaskCat.Data.Entity
{
    using Model.Vendor.ProfitSharing;

    public class VendorProfile : DbEntity
    {
        public ProfitSharingMethod Method { get; set; }
    }
}
