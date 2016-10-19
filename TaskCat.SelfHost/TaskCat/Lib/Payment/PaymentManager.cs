namespace TaskCat.Lib.Payment
{
    using System.Collections.Generic;
    using Data.Lib.Payment;
    using Manual;
    using System.Linq;

    public class PaymentManager : IPaymentManager
    {
        private IList<IPaymentMethod> paymentMethods;

        public PaymentManager()
        {
            paymentMethods = new List<IPaymentMethod>();
            paymentMethods.Add(
                new CashOnDeliveryPaymentMethod(
                    new CashOnDeliveryPaymentSettings()
                    {
                        AdditionalFee = 0,
                        IsAdditionalFeeSetOnPercentage = true,
                        TransactMode = TransactMode.AuthorizeAndCapture
                    }));
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to repost (complete) payments for redirection payment methods
        /// </summary>
        public bool AllowRePostingPayments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should bypass 'select payment method' page if we have only one payment method
        /// </summary>
        public bool BypassPaymentMethodSelectionIfOnlyOne { get; set; }

        /// <summary>
        /// Return All Available Payment Methods
        /// </summary>
        public IList<IPaymentMethod> AllPaymentMethods
        {
            get { return paymentMethods; }
        }

        /// <summary>
        /// Gets or sets a system names of active payment methods
        /// </summary>
        public IEnumerable<string> ActivePaymentMethodKeys { get { return AllPaymentMethods.Select(x => x.Key); } }
    }
}