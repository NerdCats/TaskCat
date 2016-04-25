﻿namespace TaskCat.Lib.Payment
{
    using System.Collections.Generic;
    using Data.Lib.Payment;

    public interface IPaymentManager
    {
        bool AllowRePostingPayments { get; set; }

        IList<IPaymentMethod> AllPaymentMethods { get; }
    }
}