namespace TaskCat.Data.Entity
{
    using Model.Vendor.ProfitSharing;
    using System.ComponentModel.DataAnnotations;

    public class VendorProfile : DbEntity
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Profit Sharing Strategy not provided")]
        public ProfitSharingStrategy Strategy { get; set; }
        public decimal? FixedDeliveryCharge { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "UserId not provided")]
        public string UserId { get; set; }
    }
}
