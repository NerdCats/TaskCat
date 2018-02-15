namespace TaskCat.Tests.Order
{
    using Data.Model.Inventory;
    using NUnit.Framework;
    using System.Collections.Generic;
    using TaskCat.Job.Order;

    [TestFixture(TestOf =typeof(DefaultDeliveryServiceChargeCalculationService))]
    public class DefaultDeliveryServiceChargeCalculationServiceTest
    {
        [Test]
        public void Test_CalculateServiceCharge_Weight_Greater_Than_Threshold()
        {
            var items = new List<ItemDetails>();
            items.Add(new ItemDetails { Weight = 10.0m });
            items.Add(new ItemDetails { Weight = 15.0m });

            // Act
            var deliveryChargeCalcSvc = new DefaultDeliveryServiceChargeCalculationService();
            var actual = deliveryChargeCalcSvc.CalculateServiceCharge(items);
            var expected = 150.0m;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_CalculateServiceCharge_Weight_Less_Than_Threshold()
        {
            var items = new List<ItemDetails>();
            items.Add(new ItemDetails { Weight = 0.1m });
            items.Add(new ItemDetails { Weight = 0.3m });

            // Act
            var deliveryChargeCalcSvc = new DefaultDeliveryServiceChargeCalculationService();
            var actual = deliveryChargeCalcSvc.CalculateServiceCharge(items);
            var expected = 0.0m;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
