namespace TaskCat.Payment.Exceptions
{
    using System;
    public class PaymentMethodException : Exception
    {
        public PaymentMethodException(string message) : base(message)
        {
        }

        public PaymentMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}