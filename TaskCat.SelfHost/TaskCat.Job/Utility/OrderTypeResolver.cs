namespace TaskCat.Job.Utility
{
    using Data.Model;
    using Data.Model.Order;
    using System;
    using Data.Model.Order.Delivery;

    public class OrderTypeResolver
    {  
        public static OrderModel CreateOrderInstance(string orderType)
        {
            OrderModel orderModel = default(OrderModel);
            switch (orderType)
            {
                case OrderTypes.Delivery:
                    orderModel = new DeliveryOrder();
                    break;
                case OrderTypes.ClassifiedDelivery:
                    orderModel = new ClassifiedDeliveryOrder();
                    break;
                default:
                    throw new NotSupportedException(string.Concat("Order Entry type invalid/not supported - ", orderType));
            }

            return orderModel;
        }

        public static Type ResolveOrderType(string orderType)
        {
            var order = CreateOrderInstance(orderType);
            return order.GetType();
        }
    }
}