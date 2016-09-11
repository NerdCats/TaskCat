namespace TaskCat.Tests.Order.Process
{
    using Data.Model.Geocoding;
    using Data.Model.Inventory;
    using Data.Model.Order;
    using Data.Model.Order.Delivery;
    using NUnit.Framework;
    using System.Collections.Generic;
    using TaskCat.Lib.Order;
    using TaskCat.Lib.Order.Process;

    [TestFixture(TestOf = typeof(DeliveryOrderProcessor))]
    public class DeliveryOrderProcessorTest
    {
        [Test]
        public void Test_ProcessOrder_Valid()
        {
            // Arrange
            var orderCalcSvc = new DefaultOrderCalculationService();
            var serviceChargeCalcSvc = new DefaultDeliveryServiceChargeCalculationService();

            var deliveryOrderProcessor = new DeliveryOrderProcessor(
                orderCalcSvc, serviceChargeCalcSvc);

            var deliveryOrder = new DeliveryOrder
            {
                UserId = "this.User",
                From = new DefaultAddress { Address = "My Home" },
                To = new DefaultAddress { Address = "My Office" },
                OrderCart = new OrderDetails
                {
                    PackageList = new List<ItemDetails>()
                    {
                        new ItemDetails
                        {
                            Item = "Coffee",
                            Quantity = 2,
                            Price = 150.0m,
                            VAT = 15.0m
                        }              
                    }                    
                },
                PaymentMethod = "CashOnDelivery"
            };

            // Act
            deliveryOrderProcessor.ProcessOrder(deliveryOrder);
            var coffees = deliveryOrder.OrderCart.PackageList;
            var cart = deliveryOrder.OrderCart;

            var serviceCharge = serviceChargeCalcSvc.CalculateServiceCharge(coffees);
            var subTotal = orderCalcSvc.CalculateSubtotal(coffees);
            var totalToPay = orderCalcSvc.CalculateTotalToPay(coffees, serviceCharge);
            var totalVATAmount = orderCalcSvc.CalculateTotalVATAmount(coffees, serviceCharge);
            var totalWeight = 0;


            // Assert
            Assert.AreEqual(serviceCharge, cart.ServiceCharge);
            Assert.AreEqual(subTotal, cart.SubTotal);
            Assert.AreEqual(totalToPay, cart.TotalToPay);
            Assert.AreEqual(totalVATAmount, cart.TotalVATAmount);
            Assert.AreEqual(totalWeight, cart.TotalWeight);
        }
    }
}
