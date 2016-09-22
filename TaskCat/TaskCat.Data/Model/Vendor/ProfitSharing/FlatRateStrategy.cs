namespace TaskCat.Data.Model.Vendor.ProfitSharing
{
    public class FlatRateStrategy : ProfitSharingStrategy
    {
        public decimal FlatRate { get; set; } = 50;

        public ProfitSharingMethod Method
        {
            get
            {
                return ProfitSharingMethod.FLAT_RATE;
            }
        }

        public override ProfitShareResult Calculate(decimal totalPrice)
        {
            return new ProfitShareResult()
            {
                Profit = FlatRate,
                VendorShare = totalPrice - FlatRate
            };
        }
    }
}
