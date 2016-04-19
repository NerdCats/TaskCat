namespace TaskCat.Data.Lib.Payment.Request
{
    using System.Collections.Generic;

    // TODO: Refactor this so someday I have very specific PaymentRequest
    // Currently it does have manual payment and card payment in one request
    // Someday we can break it up and make it more specific to the use case

    /// <summary>
    /// Payment Request Model
    /// </summary>
    public partial class ProcessPaymentRequest
    {
        public ProcessPaymentRequest()
        {
            this.CustomValues = new Dictionary<string, object>();
        }

        /// <summary>
        /// You can store any custom value in this property
        /// </summary>
        public Dictionary<string, object> CustomValues { get; set; }

        /// <summary>
        /// Gets or sets a store identifier
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// Gets or sets a customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets an order unique identifier. Used when order is not saved yet (payment gateways that do not redirect a customer to a third-party URL)
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// Gets or sets an order total
        /// </summary>
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// /// <summary>
        /// Gets or sets a payment method identifier
        /// </summary>
        /// </summary>
        public string PaymentMethodSystemName { get; set; }

        #region Payment method specific properties 

        /// <summary>
        /// Gets or sets a credit card type (Visa, Master Card, etc...). 
        /// We leave it empty if not used by a payment gateway
        /// </summary>
        public string CreditCardType { get; set; }

        /// <summary>
        /// Gets or sets a credit card owner name
        /// </summary>
        public string CreditCardName { get; set; }

        /// <summary>
        /// Gets or sets a credit card number
        /// </summary>
        public string CreditCardNumber { get; set; }

        /// <summary>
        /// Gets or sets a credit card expire year
        /// </summary>
        public int CreditCardExpireYear { get; set; }

        /// <summary>
        /// Gets or sets a credit card expire month
        /// </summary>
        public int CreditCardExpireMonth { get; set; }

        /// <summary>
        /// Gets or sets a credit card CVV2 (Card Verification Value)
        /// </summary>
        public string CreditCardCvv2 { get; set; }

        #endregion
    }
}