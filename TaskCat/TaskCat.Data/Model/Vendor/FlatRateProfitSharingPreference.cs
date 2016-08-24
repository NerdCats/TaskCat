namespace TaskCat.Data.Model.Vendor
{
    public class FlatRateProfitSharingPreference : IProfitSharingPreference
    {
        public decimal FlatRate { get; set; } = 50;
        public ProfitSharingMethod Method
        {
            get { return ProfitSharingMethod.FLAT_RATE; }
        }

        public decimal Calculate(decimal totalPrice) => FlatRate;
    }
}
