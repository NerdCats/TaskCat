namespace TaskCat.Lib.Payment.Manual
{
    using System;
    using System.Collections.Generic;
    using Data.Lib.Payment;
    using Data.Lib.Payment.Request;
    using Data.Lib.Payment.Response;
    using Data.Model.Inventory;
    using Data.Model.Payment;
    using Data.Entity;

    /// <summary>
    /// Manual payment processor for cash when delivery processes
    /// </summary>
    public class CashOnDeliveryPaymentMethod : IPaymentMethod
    {

        private readonly CashOnDeliveryPaymentSettings _codPaymentSettings;

        public CashOnDeliveryPaymentMethod(CashOnDeliveryPaymentSettings codPaymentSettings)
        {
            this._codPaymentSettings = codPaymentSettings;
        }

        public string Key
        {
            get
            {
                return "CashOnDelivery";
            }
        }

        public string Name
        {
            get
            {
                return "Cash On Delivery";
            }
        }

        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        public bool CanRePostProcessPayment(Job job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            //it's not a redirection payment method. So we always return false
            return false;
        }

        public CapturePaymentResponse Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResponse();
            result.AddError("Capture method not supported");
            return result;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _codPaymentSettings.AdditionalFee;
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public ProcessPaymentResponse ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResponse();

            result.AllowStoringCreditCardNumber = false;
            switch (_codPaymentSettings.TransactMode)
            {
                case TransactMode.Pending:
                    result.NewPaymentStatus = PaymentStatus.Pending;
                    break;
                case TransactMode.Authorize:
                    result.NewPaymentStatus = PaymentStatus.Authorized;
                    break;
                case TransactMode.AuthorizeAndCapture:
                    result.NewPaymentStatus = PaymentStatus.Paid;
                    break;
                default:
                    {
                        result.AddError("Not supported transaction type");
                        return result;
                    }
            }
            return result;
        }

        public RefundPaymentResponse Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResponse();
            result.AddError("Refund method not supported");
            return result;
        }

        public VoidPaymentResponse Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResponse();
            result.AddError("Void method not supported");
            return result;
        }
    }
}