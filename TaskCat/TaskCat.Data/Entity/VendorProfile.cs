namespace TaskCat.Data.Entity
{
    using Model.Vendor;

    public class VendorProfile : DbEntity
    {
        public ProfitSharingMethod Method { get; set; }
    }
}
