namespace TaskCat.Lib.Payment
{
    using System;
    using System.Collections.Generic;
    using Data.Lib.Payment;

    public class PaymentManager : IPaymentManager
    {
        public IList<IPaymentMethod> GetPaymentMethods()
        {
            throw new NotImplementedException();
        }
    }
}