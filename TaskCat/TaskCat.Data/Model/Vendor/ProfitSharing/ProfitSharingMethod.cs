namespace TaskCat.Data.Model.Vendor.ProfitSharing
{
    public enum ProfitSharingMethod
    {
        /// <summary>
        /// Depicts a scenario where the vendor pays a flat rate money for every delivery
        /// </summary>
        FLAT_RATE,
        /// <summary>
        /// Depicts a scenario where the vendor pays a percentage from the total cost
        /// </summary>
        PRICE_PERCENTAGE,
        /// <summary>
        /// Depicts a scenario where the vendor pays a percentage from the price with a total flat rate
        /// </summary>
        PRICE_PERCENTAGE_WITH_FLAT_RATE  
    }
}
