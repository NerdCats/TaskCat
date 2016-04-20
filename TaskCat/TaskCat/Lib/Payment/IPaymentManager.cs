namespace TaskCat.Lib.Payment
{
    using System.Collections.Generic;
    using TaskCat.Data.Lib.Payment;

    public interface IPaymentManager
    {
        IList<IPaymentMethod> GetPaymentMethods();
    }
}