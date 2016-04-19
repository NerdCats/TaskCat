namespace TaskCat.Data.Lib.Payment.Request
{
    using TaskCat.Data.Model;

    /// <summary>
    /// Represents a RefundPaymentResult
    /// </summary>
    public partial class RefundPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order
        /// </summary>
        public OrderModel Order { get; set; }

        /// <summary>
        /// Gets or sets an amount
        /// </summary>
        public decimal AmountToRefund { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it's a partial refund; otherwize, full refund
        /// </summary>
        public bool IsPartialRefund { get; set; }
    }

}