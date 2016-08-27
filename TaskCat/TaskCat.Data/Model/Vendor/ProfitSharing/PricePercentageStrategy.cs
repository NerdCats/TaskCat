namespace TaskCat.Data.Model.Vendor.ProfitSharing
{
    using System;

    public class PricePercentageStrategy : ProfitSharingStrategy
    {
        private int percentage;
        public int Percentage
        {
            get { return percentage; }
            set
            {
                if (percentage < 0 || percentage > 100)
                    throw new ArgumentException("Percentage is supposed to be from 0 to 100");
                percentage = value;
            }
        }

        protected internal override ProfitSharingMethod Method
        {
            get
            {
                return ProfitSharingMethod.PRICE_PERCENTAGE;
            }
        }

        protected internal override decimal Calculate(decimal totalPrice)
        {
            return totalPrice * (Percentage / 100);
        }
    }
}
