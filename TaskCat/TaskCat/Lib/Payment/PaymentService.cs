namespace TaskCat.Lib.Payment
{
    using System;
    using System.Collections.Generic;
    using Data.Lib.Payment;
    using Data.Lib.Payment.Request;
    using Data.Lib.Payment.Response;
    using Data.Model.Inventory;
    using System.Linq;
    using Data.Entity;
    using Data.Model;
    using Data.Model.Payment;

    public class PaymentService : IPaymentService
    {
        public IPaymentManager _paymentManager { get; set; }
        public PaymentSettings _paymentSettings { get; set; }

        public PaymentService(IPaymentManager paymentManager)
        {
            this._paymentManager = paymentManager;
        }
        public IList<IPaymentMethod> LoadActivePaymentMethods()
        {
            // FIXME: Currently no filters are here, so it would actually be
            // something to filter against whether it is active or not
            return _paymentManager.GetPaymentMethods();
        }

        public bool CanRePostProcessPayment(Job job)
        {
            if (job == null)
                throw new ArgumentNullException("order");

            if (!_paymentSettings.AllowRePostingPayments)
                return false;

            var paymentMethod = GetPaymentMethodByName(job.PaymentMethod);
            if (paymentMethod == null)
                return false; //Payment method couldn't be loaded (for example, was uninstalled)

            if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                return false;   //this option is available only for redirection payment methods

            if (job.Deleted)
                return false;  //do not allow for deleted orders

            if (job.State == JobState.CANCELLED)
                return false;  //do not allow for cancelled orders

            if (job.PaymentStatus != PaymentStatus.Pending)
                return false;  //payment status should be Pending

            return paymentMethod.CanRePostProcessPayment(job);
        }

        public CapturePaymentResponse Capture(CapturePaymentRequest capturePaymentRequest)
        {
            throw new NotImplementedException();
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart, string paymentMethodName)
        {
            var paymentMethod = GetPaymentMethodByName(paymentMethodName);
            return paymentMethod.GetAdditionalHandlingFee(cart);
        }

        public string GetMaskedCreditCardNumber(string creditCardNumber)
        {
            throw new NotImplementedException();
        }

        public IList<int> GetRestictedCountryIds(IPaymentMethod paymentMethod)
        {
            throw new NotImplementedException();
        }

       

        public IList<IPaymentMethod> LoadAllPaymentMethods(int storeId = 0, int filterByCountryId = 0)
        {
            throw new NotImplementedException();
        }

        public IPaymentMethod GetPaymentMethodByName(string name)
        {
            return _paymentManager.GetPaymentMethods().Where(x => x.Name == name).FirstOrDefault();
        }

        public ProcessPaymentResponse ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public ProcessPaymentResponse ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public RefundPaymentResponse Refund(RefundPaymentRequest refundPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public void SaveRestictedCountryIds(IPaymentMethod paymentMethod, List<int> countryIds)
        {
            throw new NotImplementedException();
        }

        public bool SupportCapture(string paymentMethodSystemName)
        {
            throw new NotImplementedException();
        }

        public bool SupportPartiallyRefund(string paymentMethodSystemName)
        {
            throw new NotImplementedException();
        }

        public bool SupportRefund(string paymentMethodSystemName)
        {
            throw new NotImplementedException();
        }

        public bool SupportVoid(string paymentMethodSystemName)
        {
            throw new NotImplementedException();
        }

        public VoidPaymentResponse Void(VoidPaymentRequest voidPaymentRequest)
        {
            throw new NotImplementedException();
        }


    }
}