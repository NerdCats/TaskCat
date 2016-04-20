namespace TaskCat.Data.Lib.Payment.Request
{
    using TaskCat.Data.Model;
    /// <summary>
    /// Represents a CapturePaymentRequest
    /// </summary>
    public partial class CapturePaymentRequest
    {
        /// <summary>
        /// Gets or sets an order
        /// </summary>
        public OrderModel Order { get; set; }
    }
}