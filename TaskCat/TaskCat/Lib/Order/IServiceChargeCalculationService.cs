namespace TaskCat.Lib.Order
{
    using System.Collections.Generic;
    using Data.Model.Inventory;

    public interface IServiceChargeCalculationService
    {
        decimal CalculateServiceCharge(List<ItemDetails> items);
    }
}
