namespace TaskCat.Payment.Manual
{
    public class CashOnDeliveryPaymentSettings
    {
        public TransactMode TransactMode { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool IsAdditionalFeeSetOnPercentage { get; set; }
        /// <summary>
        /// Additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }
    }
}