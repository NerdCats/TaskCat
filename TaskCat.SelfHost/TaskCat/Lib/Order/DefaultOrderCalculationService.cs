using System;

namespace TaskCat.Lib.Order
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.Model.Inventory;
    using Exceptions;

    public partial class DefaultOrderCalculationService : IOrderCalculationService
    {
        public DefaultOrderCalculationService()
        {

        }

        #region calculate
        public decimal CalculateSubtotal(List<ItemDetails> items)
        {
            return items.Sum(i => i.TotalPlusVAT);
        }

        public decimal CalculateNetTotal(List<ItemDetails> items)
        {
            return items.Sum(i => i.Total);
        }

        public decimal CalculateTotalToPay(List<ItemDetails> items, decimal serviceCharge)
        {
            return CalculateSubtotal(items) + serviceCharge;
        }

        // FIXME: I need help @.@
        public decimal CalculateTotalVATAmount(List<ItemDetails> items, decimal serviceCharge)
        {
            return CalculateSubtotal(items) - CalculateNetTotal(items);
        }
        #endregion

        #region verify
        public decimal VerifyAndCalculateSubtotal(List<ItemDetails> items, decimal submittedSubTotal)
        {
            var calculatedSubTotal =  CalculateSubtotal(items);
            if (calculatedSubTotal == submittedSubTotal) return submittedSubTotal;
            throw new OrderCalculationException("SubTotal");
        }

        public decimal VerifyAndCalculateNetTotal(List<ItemDetails> items, decimal submittedNetTotal)
        {
            var calculatedNetTotal = CalculateNetTotal(items);
            if (submittedNetTotal == calculatedNetTotal) return submittedNetTotal;
            throw new OrderCalculationException("NetTotal");
        }

        public decimal VerifyAndCalculateTotalToPay(List<ItemDetails> items, decimal serviceCharge, decimal submittedTotalToPay)
        {
            var calculatedTotalToPay = CalculateTotalToPay(items, serviceCharge);
            if (calculatedTotalToPay == submittedTotalToPay) return submittedTotalToPay;
            throw new OrderCalculationException("TotalToPay");
        }

        // FIXME: I need help @.@
        public decimal VerifyAndTotalVATAmount(List<ItemDetails> items, decimal serviceCharge, decimal submittedTotalVatAmount)
        {
            var calculatedTotalVatAmount = CalculateTotalVATAmount(items, serviceCharge);
            if (calculatedTotalVatAmount == submittedTotalVatAmount) return submittedTotalVatAmount;
            throw new OrderCalculationException("TotalVATAmount");
        }
        #endregion
    }
}