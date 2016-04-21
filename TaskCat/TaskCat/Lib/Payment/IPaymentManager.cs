namespace TaskCat.Lib.Payment
{
    using System.Collections.Generic;
    using Data.Lib.Payment;

    public interface IPaymentManager
    {
        IList<IPaymentMethod> GetPaymentMethods();
    }
}