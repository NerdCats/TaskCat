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
    using System.Text;
    using Exceptions;

    /// <summary>
    /// Default Payment Service for all installer services through a IPaymentManager 
    /// </summary>
    public class PaymentService : IPaymentService
    {
        public IPaymentManager paymentManager { get; set; }

        public PaymentService(IPaymentManager paymentManager)
        {
            this.paymentManager = paymentManager;
        }

        public IList<IPaymentMethod> GetActivePaymentMethods()
        {
            // FIXME: Currently no filters are here, so it would actually be
            // something to filter against whether it is active or not
            return paymentManager.AllPaymentMethods;
        }

        public IList<IPaymentMethod> GetAllPaymentMethods(int storeId = 0, int filterByCountryId = 0)
        {
            // FIXME: When Country restrictions come here, we have to use here
            return paymentManager.AllPaymentMethods;
        }

        public IPaymentMethod GetPaymentMethodByKey(string name)
        {
            var method = paymentManager.AllPaymentMethods.Where(x => x.Key == name).FirstOrDefault();
            if (method == null)
                throw new PaymentMethodException("Invalid payment method, no payment method named " + name + " not found");
            return method;
        }

        public ProcessPaymentResponse ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest.OrderTotal == decimal.Zero)
            {
                var result = new ProcessPaymentResponse
                {
                    NewPaymentStatus = PaymentStatus.Paid
                };
                return result;
            }

            //We should strip out any white space or dash in the CC number entered.
            if (!string.IsNullOrWhiteSpace(processPaymentRequest.CreditCardNumber))
            {
                processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace(" ", "");
                processPaymentRequest.CreditCardNumber = processPaymentRequest.CreditCardNumber.Replace("-", "");
            }
            var paymentMethod = GetPaymentMethodByKey(processPaymentRequest.PaymentMethodName);
            return paymentMethod.ProcessPayment(processPaymentRequest);
        }

        public bool CanRePostProcessPayment(Job job)
        {
            if (job == null)
                throw new ArgumentNullException("order");

            if (!paymentManager.AllowRePostingPayments)
                return false;

            var paymentMethod = job.PaymentMethod;

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

        public bool SupportCapture(string paymentMethodName)
        {
            var paymentMethod = GetPaymentMethodByKey(paymentMethodName);
            return paymentMethod.SupportCapture;
        }

        public CapturePaymentResponse Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var paymentMethod = GetPaymentMethodByKey(capturePaymentRequest.Order.PaymentMethod);
            return paymentMethod.Capture(capturePaymentRequest);
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart, string paymentMethodName)
        {
            var paymentMethod = GetPaymentMethodByKey(paymentMethodName);
            return paymentMethod.GetAdditionalHandlingFee(cart);
        }

        public string GetMaskedCreditCardNumber(string creditCardNumber)
        {
            if (string.IsNullOrEmpty(creditCardNumber))
                throw new ArgumentNullException("credit card number provided null");

            string last4 = creditCardNumber.Substring(creditCardNumber.Length - 4, 4);

            StringBuilder stringBuilder = new StringBuilder();           
            for (int i = 0; i < creditCardNumber.Length - 4; i++)
            {
                stringBuilder.Append("X");
            }
            return string.Concat(stringBuilder.ToString(), last4);
        }

        public IList<int> GetRestrictedCountryIds(IPaymentMethod paymentMethod)
        {
            // TODO: What we have to do here is install a settings service that will tell us 
            // that which countries are restricted from payment

            throw new NotImplementedException();
        }

        public void SaveRestrictedCountryIds(IPaymentMethod paymentMethod, List<int> countryIds)
        {
            throw new NotImplementedException();
        }

        public bool SupportRefund(string paymentMethodSystemName)
        {
            var paymentMethod = GetPaymentMethodByKey(paymentMethodSystemName);
            return paymentMethod.SupportRefund;
        }

        public RefundPaymentResponse Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var paymentMethod = GetPaymentMethodByKey(refundPaymentRequest.Order.PaymentMethod);
            return paymentMethod.Refund(refundPaymentRequest);
        }

        public bool SupportPartiallyRefund(string paymentMethodName)
        {
            var paymentMethod = GetPaymentMethodByKey(paymentMethodName);
            return paymentMethod.SupportPartiallyRefund;
        }       

        public bool SupportVoid(string paymentMethodName)
        {
            var paymentMethod = GetPaymentMethodByKey(paymentMethodName);
            return paymentMethod.SupportVoid;
        }

        public VoidPaymentResponse Void(VoidPaymentRequest voidPaymentRequest)
        {
            var paymentMethod = GetPaymentMethodByKey(voidPaymentRequest.Order.PaymentMethod);
            return paymentMethod.Void(voidPaymentRequest);
        }
    }
}