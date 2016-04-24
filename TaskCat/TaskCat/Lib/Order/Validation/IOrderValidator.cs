namespace TaskCat.Lib.Order.Validation
{
    using Data.Model;

    public interface IOrderValidator
    {
        void ValidateOrder(OrderModel order);
    }
}