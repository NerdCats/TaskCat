namespace TaskCat.Data.Model.Vendor
{
    using System;
    public class PricePercentageProfitSharingPreference : IProfitSharingPreference
    {
        private int pricePercentage;
        public int PricePercentage
        {
            get { return pricePercentage; }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("Price percentage can only be from 0 to 100");
                }
                pricePercentage = value;
            }
        }
        public ProfitSharingMethod Method
        {
            get
            {
                return ProfitSharingMethod.PRICE_PERCENTAGE;
            }
        }

        public decimal Calculate(decimal totalPrice)
        {
            return totalPrice * (PricePercentage / 100);
        }
    }
}
