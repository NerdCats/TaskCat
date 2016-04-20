namespace TaskCat.Data.Lib.Payment.Response
{
    using System.Collections.Generic;
    using TaskCat.Data.Model.Payment;

    /// <summary>
    /// Capture payment result
    /// </summary>
    public partial class CapturePaymentResponse
    {
        private PaymentStatus _newPaymentStatus = PaymentStatus.Pending;

        /// <summary>
        /// Ctor
        /// </summary>
        public CapturePaymentResponse()
        {
            // TODO: I really dont like having an ERRORS property, its okay per say but Id always
            // prefer exceptions over this
            this.Errors = new List<string>();
        }

        public List<string> Errors { get; private set; }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return (this.Errors.Count == 0); }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            this.Errors.Add(error);
        }

        /// <summary>
        /// Gets or sets the capture transaction identifier
        /// </summary>
        public string CaptureTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the capture transaction result
        /// </summary>
        public string CaptureTransactionResult { get; set; }

        /// <summary>
        /// Gets or sets a payment status after processing
        /// </summary>
        public PaymentStatus NewPaymentStatus
        {
            get
            {
                return _newPaymentStatus;
            }
            set
            {
                _newPaymentStatus = value;
            }
        }
    }
}