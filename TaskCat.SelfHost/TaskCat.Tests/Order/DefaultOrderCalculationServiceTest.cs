namespace TaskCat.Tests.Order
{
    using Data.Model.Inventory;
    using NUnit.Framework;
    using System.Collections.Generic;
    using TaskCat.Lib.Exceptions;
    using TaskCat.Lib.Order;

    [TestFixture(TestOf = typeof(DefaultOrderCalculationService))]
    public class DefaultOrderCalculationServiceTest
    {
        [Test]
        public void Test_CalculateSubtotal_Valid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m, VAT = 15 });

            var orderCalcSvc = new DefaultOrderCalculationService();

            // Act
            var expected = 0.0m;
            foreach (var item in items)
            {
                var total = item.Quantity * item.Price;
                var totalPlusVat = total * (1 + item.VAT / 100);
                expected += totalPlusVat;
            }

            var actual = orderCalcSvc.CalculateSubtotal(items);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_CalculateNetTotal_Valid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m });

            var orderCalcSvc = new DefaultOrderCalculationService();

            // Act
            var expected = 0.0m;
            foreach (var item in items)
            {
                expected += (item.Price * item.Quantity);
            }
            var actual = orderCalcSvc.CalculateNetTotal(items);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_CalculateTotalToPay_Valid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m, VAT = 15 });

            var orderCalcSvc = new DefaultOrderCalculationService();
            decimal serviceCharge = 100.0m;

            // Act
            var expected = 0.0m;
            foreach (var item in items)
            {
                var total = item.Quantity * item.Price;
                var totalPlusVat = total * (1 + item.VAT / 100);
                expected += totalPlusVat;
            }
            expected += serviceCharge;

            var actual = orderCalcSvc.CalculateTotalToPay(items, serviceCharge);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // FIXME: I need help @.@
        [Test]
        public void Test_CalculateTotaVATAmount_Valid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m, VAT = 15 });

            var orderCalcSvc = new DefaultOrderCalculationService();
            var subTotal = 0.0m;
            var netTotal = 0.0m;

            // Act
            foreach (var item in items)
            {
                var total = item.Quantity * item.Price;
                var totalPlusVat = total * (1 + item.VAT / 100);
                subTotal += totalPlusVat;
                netTotal += total;
            }
            var expected = subTotal - netTotal;
            var actual = orderCalcSvc.CalculateTotalVATAmount(items, 10.0m);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_VerifyAndCalculateSubtotal_Valid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m, VAT = 15 });

            var orderCalcSvc = new DefaultOrderCalculationService();
            var expected = 0.0m;

            // Act
            foreach (var item in items)
            {
                var total = item.Quantity * item.Price;
                var totalPlusVat = total * (1 + item.VAT / 100);
                expected += totalPlusVat;
            }
            var submittedSubTotal = expected;
            var actual = orderCalcSvc.VerifyAndCalculateSubtotal(items, submittedSubTotal);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_VerifyAndCalculateSubtotal_Invalid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m, VAT = 15 });

            var orderCalcSvc = new DefaultOrderCalculationService();
            var expected = 0.0m;

            // Act
            foreach (var item in items)
            {
                var total = item.Quantity * item.Price;
                var totalPlusVat = total * (1 + item.VAT / 100);
                expected += totalPlusVat;
            }
            var submittedSubTotal = 0.0m;

            // Assert
            Assert.Throws<OrderCalculationException>(
                () => orderCalcSvc.VerifyAndCalculateSubtotal(items, submittedSubTotal));
        }

        [Test]
        public void Test_VerifyAndCalculateNetTotal_Valid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m });

            var orderCalcSvc = new DefaultOrderCalculationService();
            var expected = 0.0m;

            // Act
            foreach (var item in items)
            {
                expected += (item.Price * item.Quantity);
            }
            var submittedNetTotal = expected;
            var actual = orderCalcSvc.VerifyAndCalculateNetTotal(items, submittedNetTotal);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_VerifyAndCalculateNetTotal_Invalid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m });

            var orderCalcSvc = new DefaultOrderCalculationService();
            var expected = 0.0m;

            // Act
            foreach (var item in items)
            {
                expected += (item.Price * item.Quantity);
            }
            var submittedNetTotal = 0.0m;

            // Assert
            Assert.Throws<OrderCalculationException>(
                () => orderCalcSvc.VerifyAndCalculateNetTotal(items, submittedNetTotal));
        }

        [Test]
        public void Test_VerifyAndCalculateTotalToPay_Valid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m, VAT = 15 });

            var orderCalcSvc = new DefaultOrderCalculationService();
            var expected = 0.0m;
            decimal serviceCharge = 100.0m;

            // Act
            foreach (var item in items)
            {
                var total = item.Quantity * item.Price;
                var totalPlusVat = total * (1 + item.VAT / 100);
                expected += totalPlusVat;
            }
            expected += serviceCharge;
            var submittedTotalToPay = expected;
            var actual = orderCalcSvc.VerifyAndCalculateTotalToPay(
                items, serviceCharge, submittedTotalToPay);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_VerifyAndCalculateTotalToPay_Invalid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m, VAT = 15 });

            var orderCalcSvc = new DefaultOrderCalculationService();
            var expected = 0.0m;
            decimal serviceCharge = 0.0m;

            // Act
            foreach (var item in items)
            {
                var total = item.Quantity * item.Price;
                var totalPlusVat = total * (1 + item.VAT / 100);
                expected += totalPlusVat;
            }
            expected += serviceCharge;
            var submittedTotalToPay = 0.0m;

            // Assert
            Assert.Throws<OrderCalculationException>(
                () => orderCalcSvc.VerifyAndCalculateTotalToPay(
                    items, serviceCharge, submittedTotalToPay));
        }

        // FIXME: I need help @.@
        [Test]
        public void Test_VerifyAndCalculateTotalVATAmount_Valid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m, VAT = 15 });

            var orderCalcSvc = new DefaultOrderCalculationService();
            var subTotal = 0.0m;
            var netTotal = 0.0m;

            // Act
            foreach (var item in items)
            {
                var total = item.Quantity * item.Price;
                var totalPlusVat = total * (1 + item.VAT / 100);
                subTotal += totalPlusVat;
                netTotal += total;
            }
            var expected = subTotal - netTotal;
            var submittedTotalVATAmount = expected;
            var actual = orderCalcSvc.VerifyAndTotalVATAmount(
                items, 10.0m, submittedTotalVATAmount);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_VerifyAndCalculateTotalVATAmount_Invalid()
        {
            // Arrange
            List<ItemDetails> items = new List<ItemDetails>();
            items.Add(new ItemDetails { Quantity = 5, Price = 10.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 2, Price = 100.0m, VAT = 15 });
            items.Add(new ItemDetails { Quantity = 10, Price = 15.0m, VAT = 15 });

            var orderCalcSvc = new DefaultOrderCalculationService();
            var subTotal = 0.0m;
            var netTotal = 0.0m;

            // Act
            foreach (var item in items)
            {
                var total = item.Quantity * item.Price;
                var totalPlusVat = total * (1 + item.VAT / 100);
                subTotal += totalPlusVat;
                netTotal += total;
            }
            var expected = subTotal - netTotal;
            var submittedTotalVATAmount = 0.0m;

            // Assert
            Assert.Throws<OrderCalculationException>(
                () => orderCalcSvc.VerifyAndTotalVATAmount(
                    items, 10.0m, submittedTotalVATAmount));
        }
    }
}
