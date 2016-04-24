namespace TaskCat.Tests.Order
{
    using Data.Model.Order;
    using Data.Model.Inventory;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class OrderDetailsTest
    {
        [Test]
        public void Test_Valid_OrderDetails_Creation()
        {
            OrderDetails orderDetails = new OrderDetails();
            orderDetails.PackageList = new List<ItemDetails>();

            for (int i = 0; i < 10; i++)
            {
                orderDetails.PackageList.Add(GetRandomItem());
            }

            Assert.AreEqual(10, orderDetails.PackageList.Count);
        }

        private ItemDetails GetRandomItem()
        {
            ItemDetails item = new ItemDetails();
            item.Item = Guid.NewGuid().ToString();
            Random rnd = new Random();
            item.Quantity = rnd.Next(100000);
            item.Price = rnd.Next(999999999);
            item.VAT = rnd.Next(100);
            item.Weight = rnd.Next(100);

            return item;
        }
    }
}
