namespace TaskCat.Data.Model.Vendor
{
    using System;

    public class PricePercentageProfitSharingPreference : ProfitSharingPreference
    {
        public int Percentage { get; set; }

        protected internal override ProfitSharingMethod Method
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected internal override decimal Calculate(decimal totalPrice)
        {
            return totalPrice * Percentage;
        }
    }
}
