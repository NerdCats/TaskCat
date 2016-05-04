namespace TaskCat.Data.Model.Payment
{
    /// <summary>
    /// Represents a payment status enumeration
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending,
        /// <summary>
        /// Authorized
        /// </summary>
        Authorized,
        /// <summary>
        /// Paid
        /// </summary>
        Paid,
        /// <summary>
        /// Partially Refunded
        /// </summary>
        PartiallyRefunded,
        /// <summary>
        /// Refunded
        /// </summary>
        Refunded,
        /// <summary>
        /// Voided
        /// </summary>
        Voided,
    }
}
