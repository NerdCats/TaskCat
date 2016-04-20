namespace TaskCat.Data.Lib.Payment.Request
{
    using TaskCat.Data.Model;
    /// <summary>
    /// Represents a VoidPaymentResult
    /// </summary>
    public partial class VoidPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order
        /// </summary>
        public OrderModel Order { get; set; }
    }

}