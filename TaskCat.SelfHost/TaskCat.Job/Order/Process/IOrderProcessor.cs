namespace TaskCat.Job.Order.Process
{
    using Data.Model;

    public interface IOrderProcessor
    {
        void ProcessOrder(OrderModel order);
    }
}