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

        public new ProfitSharingMethod Method => ProfitSharingMethod.PRICE_PERCENTAGE;

        public override ProfitShareResult Calculate(decimal totalPrice)
        {
            var profit = totalPrice * (Percentage / 100);
            return new ProfitShareResult()
            {
                Profit = profit,
                VendorShare = totalPrice - profit
            };
        }
    }
}
