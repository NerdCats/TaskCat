namespace TaskCat.Job.Order.Process
{
    using System.Linq;
    using Data.Model;
    using Data.Model.Order;
    using Data.Model.Inventory;
    using System.Collections.Generic;
    using Data.Model.Order.Delivery;
    using System.ComponentModel.DataAnnotations;

    public class DeliveryOrderProcessor : IOrderProcessor
    {
        private IOrderCalculationService calculationService;
        private IServiceChargeCalculationService serviceChargeCalculationService;

        public DeliveryOrderProcessor(
            IOrderCalculationService calculationService, 
            IServiceChargeCalculationService serviceChargeCalculationService)
        {
            this.calculationService = calculationService;
            this.serviceChargeCalculationService = serviceChargeCalculationService;
        }

        public void ProcessOrder(OrderModel order)
        {
            var orderModel = order as DeliveryOrder;
            Validator.ValidateObject(orderModel, new ValidationContext(orderModel), true);

            if (orderModel.OrderCart != null)
            {
                Validator.ValidateObject(orderModel.OrderCart, new ValidationContext(orderModel.OrderCart), true);
            }
            
            if (orderModel.OrderCart == null)
            {
                orderModel.OrderCart = new OrderDetails();
                orderModel.OrderCart.PackageList = new List<ItemDetails>();
            }

            var cart = orderModel.OrderCart;

            if (cart.ServiceCharge == null)
            {
                cart.ServiceCharge = serviceChargeCalculationService.
                    CalculateServiceCharge(cart.PackageList);
            }

            if (cart.SubTotal == null || cart.SubTotal.Value == 0)
            {
                cart.SubTotal = calculationService.CalculateSubtotal(cart.PackageList);
            } 
            else
            {
                cart.SubTotal = calculationService.
                    VerifyAndCalculateSubtotal(cart.PackageList, cart.SubTotal.Value);
            }

            if (cart.TotalToPay == null || cart.TotalToPay.Value == 0)
            {
                cart.TotalToPay = calculationService.
                    CalculateTotalToPay(cart.PackageList, cart.ServiceCharge.Value);
            }
            else
            {
                cart.TotalToPay = calculationService.
                    VerifyAndCalculateTotalToPay(
                    cart.PackageList, cart.ServiceCharge.Value, cart.TotalToPay.Value);
            }

            if (cart.TotalVATAmount == null || cart.TotalVATAmount.Value == 0)
            {
                cart.TotalVATAmount = calculationService.
                    CalculateTotalVATAmount(cart.PackageList, cart.ServiceCharge.Value);
            }
            else
            {
                // FIXME
                cart.TotalToPay = calculationService.
                    VerifyAndTotalVATAmount(
                    cart.PackageList, cart.ServiceCharge.Value, cart.TotalVATAmount.Value);
            }

            if (cart.TotalWeight == null || cart.TotalWeight.Value == 0)
            {
                cart.TotalWeight = cart.PackageList.Sum(x => x.Weight);
            }
        }
    }
}