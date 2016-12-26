namespace TaskCat.Job.Order
{
    using System.Collections.Generic;
    using Data.Model.Inventory;

    public interface IServiceChargeCalculationService
    {
        decimal CalculateServiceCharge(List<ItemDetails> items);
    }
}
