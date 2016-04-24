namespace TaskCat.Lib.Order.Validation
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Data.Model;
    using Data.Model.Order;

    public class DeliveryOrderValidator : IOrderValidator
    {
        private IOrderCalculationService calculationService;
        private IServiceChargeCalculationService serviceChargeCalculationService;

        public DeliveryOrderValidator(IOrderCalculationService calculationService, IServiceChargeCalculationService serviceChargeCalculationService)
        {
            this.calculationService = calculationService;
            this.serviceChargeCalculationService = serviceChargeCalculationService;
        }

        public void ValidateOrder(OrderModel order)
        {
            var orderModel = order as DeliveryOrder;
            Validator.ValidateObject(orderModel, new ValidationContext(orderModel), true);
            Validator.ValidateObject(orderModel.OrderCart, new ValidationContext(orderModel.OrderCart), true);

            var cart = orderModel.OrderCart;

            if (cart.ServiceCharge == null || cart.ServiceCharge.Value == 0)
            {
                cart.ServiceCharge = serviceChargeCalculationService.CalculateServiceCharge(cart.PackageList);
            }

            if (cart.SubTotal == null || cart.SubTotal.Value == 0)
            {
                cart.SubTotal = calculationService.CalculateSubtotal(cart.PackageList);
            }
            else
            {
                cart.SubTotal = calculationService.VerifyAndCalculateSubtotal(cart.PackageList, cart.SubTotal.Value);
            }

            if (cart.TotalToPay == null || cart.TotalToPay.Value == 0)
            {
                cart.TotalToPay = calculationService.CalculateTotalToPay(cart.PackageList, cart.ServiceCharge.Value);
            }
            else
            {
                cart.TotalToPay = calculationService.VerifyAndCalculateTotalToPay(cart.PackageList, cart.ServiceCharge.Value, cart.TotalToPay.Value);
            }

            if (cart.TotalVATAmount == null || cart.TotalVATAmount.Value == 0)
            {
                cart.TotalVATAmount = calculationService.CalculateTotalVATAmount(cart.PackageList, cart.ServiceCharge.Value);
            }
            else
            {
                cart.TotalToPay = calculationService.VerifyAndTotalVATAmount(cart.PackageList, cart.ServiceCharge.Value, cart.TotalVATAmount.Value);
            }

            if (cart.TotalWeight == null || cart.TotalWeight.Value == 0)
            {
                cart.TotalWeight = cart.PackageList.Sum(x => x.Weight);
            }
        }
    }
}