namespace TaskCat.Tests.Model.Geocoding
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using Data.Model.Geocoding;
    using Data.Model.GeoJson;

    [TestFixture(TestOf =typeof(DefaultAddress))]
    public class DefaultAddressTests
    {
        DefaultAddress addressWithFormattedString;
        DefaultAddress address;

        [Test]
        public void Test_DefaultAddress_Creation()
        {
            addressWithFormattedString = new DefaultAddress("Test Formatted Address", new Point(new double[] { 1, 2 }.ToList()));
            Assert.AreEqual("Test Formatted Address", addressWithFormattedString.Address);

            address = new DefaultAddress("Test AddressLine 1", "Test AddressLine 2", "Testlocality", "Dhaka", "1217", "Bangladesh", new Point(new double[] { 1, 2 }.ToList()));
            address.State = "Dhaka";
            Assert.AreEqual("Test AddressLine 1", address.AddressLine1);
            Assert.AreEqual("Test AddressLine 2", address.AddressLine2);
            Assert.AreEqual("Dhaka", address.City);
            Assert.AreEqual("1217", address.PostalCode);
            Assert.AreEqual("Bangladesh", address.Country);
            Assert.AreEqual("Test AddressLine 1, Test AddressLine 2, Testlocality, Dhaka-1217, Dhaka, Bangladesh", address.Address);
        }

        [Test]
        public void Test_DefaultAddress_Creation_Without_AddressLine()
        {
            Assert.Throws<ArgumentException>(()=> {
                address = new DefaultAddress("", "Test AddressLine 2", "Testlocality", "Dhaka", "1217", "Bangladesh", new Point(new double[] { 1, 2 }.ToList()));
            });            
        }
    }
}