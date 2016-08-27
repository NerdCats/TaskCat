namespace TaskCat.Data.Model.Vendor.ProfitSharing
{
    public class FlatRateStrategy : ProfitSharingStrategy
    {
        public decimal FlatRate { get; set; } = 50;

        protected internal override ProfitSharingMethod Method
        {
            get
            {
                return ProfitSharingMethod.FLAT_RATE;
            }
        }

        protected internal override decimal Calculate(decimal totalPrice)
        {
            return FlatRate;
        }
    }
}
