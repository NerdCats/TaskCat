namespace TaskCat.Lib.Order.Process
{
    using Data.Model;

    public interface IOrderProcessor
    {
        void ValidateOrder(OrderModel order);
    }
}