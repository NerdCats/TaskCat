namespace TaskCat.Lib.Payment
{
    using System.Collections.Generic;
    using Data.Lib.Payment;
    using Manual;

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
                        AdditionalFee = 2,
                        IsAdditionalFeeSetOnPercentage = true,
                        TransactMode = TransactMode.AuthorizeAndCapture
                    }));
        }

        public IList<IPaymentMethod> GetPaymentMethods()
        {
            return paymentMethods;
        }
    }
}