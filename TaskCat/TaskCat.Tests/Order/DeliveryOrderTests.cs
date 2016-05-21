namespace TaskCat.Tests.Order
{
    using NUnit.Framework;
    using Data.Model.Order;
    using Data.Model.GeoJson;
    using Data.Lib.Constants;
    using System.Linq;
    using System.Collections.Generic;
    using Data.Model.Geocoding;
    using Data.Model.Inventory;

    [TestFixture(TestOf = typeof(DeliveryOrder))]
    public class DeliveryOrderTests
    {
        [Test]
        public void Test_DeliveryOrder_Creation_Without_Name()
        {
            DefaultAddress FromLocation = new DefaultAddress("Test From Address", new Point((new double[] { 1, 2 }).ToList()));
            DefaultAddress ToLocation = new DefaultAddress("Test To Address", new Point((new double[] { 2, 1 }).ToList()));
            string PackageDescription = "Sample Package Description";

            List<ItemDetails> invoiceItems = new List<ItemDetails>();
            invoiceItems.Add(new ItemDetails()
            {
                Item = "Test Item 1",
                Price = 100,
                Quantity = 1,
                VAT = 10,
                Weight = 5
            });

            invoiceItems.Add(new ItemDetails()
            {
                Item = "Test Item 2",
                Price = 100,
                Quantity = 3,
                VAT = 20,
                Weight = 2
            });

            DeliveryOrder order = new DeliveryOrder();
            order.From = FromLocation;
            order.To = ToLocation;
            order.Description = PackageDescription;
            order.UserId = "12345678";

            Assert.IsNotNull(order);
            Assert.AreEqual(FromLocation, order.From);
            Assert.AreEqual(ToLocation, order.To);
            Assert.AreEqual(null, order.Name);
            Assert.AreEqual(PackageDescription, order.Description);
            Assert.AreEqual("default", order.PayloadType);
            Assert.AreEqual(JobTaskTypes.DELIVERY, order.Type);
            Assert.Null(order.ETA);
            Assert.Null(order.ETAMinutes);
            Assert.AreEqual("12345678", order.UserId);
        }

        [Test]
        public void Test_DeliveryOrder_Creation_With_Name()
        {
            DefaultAddress FromLocation = new DefaultAddress("Test From Address", new Point((new double[] { 1, 2 }).ToList()));
            DefaultAddress ToLocation = new DefaultAddress("Test To Address", new Point((new double[] { 2, 1 }).ToList()));
            string orderName = "Test Delivery Order";
            string PackageDescription = "Sample Package Description";

            List<ItemDetails> invoiceItems = new List<ItemDetails>();
            invoiceItems.Add(new ItemDetails()
            {
                Item = "Test Item 1",
                Price = 100,
                Quantity = 1,
                VAT = 10,
                Weight = 5
            });

            invoiceItems.Add(new ItemDetails()
            {
                Item = "Test Item 2",
                Price = 100,
                Quantity = 3,
                VAT = 20,
                Weight = 2
            });

            DeliveryOrder order = new DeliveryOrder(orderName);
            order.From = FromLocation;
            order.To = ToLocation;
            order.Description = PackageDescription;
            order.UserId = "12345678";

            Assert.IsNotNull(order);
            Assert.AreEqual(FromLocation, order.From);
            Assert.AreEqual(ToLocation, order.To);
            Assert.AreEqual(orderName, order.Name);
            Assert.AreEqual("default", order.PayloadType);
            Assert.AreEqual(JobTaskTypes.DELIVERY, order.Type);
            Assert.Null(order.ETA);
            Assert.Null(order.ETAMinutes);
            Assert.AreEqual("12345678", order.UserId);
        }


    }
}
