namespace TaskCat.Tests.Order
{
    using NUnit.Framework;
    using Data.Model.Order;
    using Data.Model;
    using Data.Model.GeoJson;
    using Data.Lib.Constants;
    using System.Linq;
    using Data.Model.Invoice;
    using System.Collections.Generic;

    [TestFixture(TestOf = typeof(DeliveryOrder))]
    public class DeliveryOrderTests
    {

        [Test]
        public void Test_DeliveryOrder_Creation_Without_Name()
        {
            Location FromLocation = new Location() { Address = "Test From Address", Point = new Point((new double[] { 1, 2 }).ToList()) };

            Location ToLocation = new Location() { Address = "Test To Address", Point = new Point((new double[] { 2, 1 }).ToList()) };
            string PackageDescription = "Sample Package Description";

            List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
            invoiceItems.Add(new InvoiceItem()
            {
                Item = "Test Item 1",
                Price = 100,
                Quantity = 1,
                VAT = 10,
                Weight = 5
            });

            invoiceItems.Add(new InvoiceItem()
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
            order.PackageDescription = PackageDescription;
            order.PackageList = invoiceItems;

            Assert.IsNotNull(order);
            Assert.AreEqual(FromLocation, order.From);
            Assert.AreEqual(ToLocation, order.To);
            Assert.IsTrue(order.Name.StartsWith(string.Concat(JobTaskTypes.DELIVERY, " Request for ", "Anonymous", " at ")));
            Assert.AreEqual(PackageDescription, order.PackageDescription);
            Assert.AreEqual(invoiceItems.Sum(x => x.Weight), order.PackageWeight);
            Assert.AreEqual("default", order.PayloadType);
            Assert.AreEqual(JobTaskTypes.DELIVERY, order.Type);
            Assert.Null(order.ETA);
            Assert.Null(order.ETAMinutes);
            Assert.AreEqual("Anonymous", order.User);
        }

        [Test]
        public void Test_DeliveryOrder_Creation_With_Name()
        {
            Location FromLocation = new Location() { Address = "Test From Address", Point = new Point((new double[] { 1, 2 }).ToList()) };

            Location ToLocation = new Location() { Address = "Test To Address", Point = new Point((new double[] { 2, 1 }).ToList()) };
            string orderName = "Test Delivery Order";
            string PackageDescription = "Sample Package Description";

            List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
            invoiceItems.Add(new InvoiceItem()
            {
                Item = "Test Item 1",
                Price = 100,
                Quantity = 1,
                VAT = 10,
                Weight = 5
            });

            invoiceItems.Add(new InvoiceItem()
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
            order.PackageDescription = PackageDescription;
            order.PackageList = invoiceItems;

            Assert.IsNotNull(order);
            Assert.AreEqual(FromLocation, order.From);
            Assert.AreEqual(ToLocation, order.To);
            Assert.AreEqual(orderName, order.Name);
            Assert.AreEqual(PackageDescription, order.PackageDescription);
            Assert.AreEqual(invoiceItems.Sum(x => x.Weight), order.PackageWeight);
            Assert.AreEqual("default", order.PayloadType);
            Assert.AreEqual(JobTaskTypes.DELIVERY, order.Type);
            Assert.Null(order.ETA);
            Assert.Null(order.ETAMinutes);
            Assert.AreEqual("Anonymous", order.User);
        }


    }
}
