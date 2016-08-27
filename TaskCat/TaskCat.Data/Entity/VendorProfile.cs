namespace TaskCat.Data.Entity
{
    using Model.Vendor.ProfitSharing;

    public class VendorProfile : DbEntity
    {
        public ProfitSharingStrategy Strategy { get; set; }
        public decimal FixedDeliveryCharge { get; set; }
    }
}
