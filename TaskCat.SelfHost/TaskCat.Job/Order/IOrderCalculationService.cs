namespace TaskCat.Job.Order
{
    using TaskCat.Data.Model.Inventory;
    using System.Collections.Generic;

    public interface IOrderCalculationService
    {
        decimal CalculateNetTotal(List<ItemDetails> items);
        decimal CalculateSubtotal(List<ItemDetails> items);
        decimal CalculateTotalToPay(List<ItemDetails> items, decimal serviceCharge);
        decimal CalculateTotalVATAmount(List<ItemDetails> items, decimal serviceCharge);
        decimal VerifyAndCalculateNetTotal(List<ItemDetails> items, decimal submittedNetTotal);
        decimal VerifyAndCalculateSubtotal(List<ItemDetails> items, decimal submittedSubTotal);
        decimal VerifyAndCalculateTotalToPay(List<ItemDetails> items, decimal serviceCharge, decimal submittedTotalToPay);
        decimal VerifyAndTotalVATAmount(List<ItemDetails> items, decimal serviceCharge, decimal submittedTotalVatAmount);
    }
}