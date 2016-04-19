namespace TaskCat.Data.Model.Invoice.Tests.Model.Invoice
{
    using Inventory;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [TestFixture(TestOf = typeof(ItemDetails))]
    public class InvoiceItemTests
    {
        [Test]
        public void Test_Create_InvoiceItem()
        {
            ItemDetails invoiceItem = new ItemDetails();

            Assert.NotNull(invoiceItem);
        }

        [Test]
        public void Test_Create_InvoiceItem_Without_Item()
        {
            Assert.Throws(typeof(ValidationException), () =>
            {
                ItemDetails invoiceItem = new ItemDetails();

                Assert.NotNull(invoiceItem);
                Validator.ValidateObject(invoiceItem, new ValidationContext(invoiceItem));
            });
        }

        [Test]
        public void Test_Create_InvoiceItem_With_Item()
        {
            Assert.DoesNotThrow(() =>
            {
                ItemDetails invoiceItem = new ItemDetails();
                invoiceItem.Item = "Test Item";

                Assert.NotNull(invoiceItem);
                Assert.NotNull(invoiceItem.Item);
                Assert.AreEqual("Test Item", invoiceItem.Item);

                Validator.ValidateObject(invoiceItem, new ValidationContext(invoiceItem));
            });
        }

        [Test]
        public void Test_Create_InvoiceItem_Quantity_WrongRange()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ItemDetails invoiceItem = new ItemDetails();
            invoiceItem.Item = "Test Item";
            invoiceItem.Quantity = 0;
            invoiceItem.Price = 10;

            Assert.IsFalse(Validator.TryValidateObject(invoiceItem, new ValidationContext(invoiceItem), validationResults, true));

            invoiceItem.Quantity = 2;
            Assert.IsTrue(Validator.TryValidateObject(invoiceItem, new ValidationContext(invoiceItem), validationResults, true));
            Assert.AreEqual(2, invoiceItem.Quantity);
        }

        [Test]
        public void Test_Create_InvoiceItem_Price_Wrong_Range()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ItemDetails invoiceItem = new ItemDetails();
            invoiceItem.Item = "Test Item";
            invoiceItem.Quantity = 1;

            Validator.ValidateObject(invoiceItem, new ValidationContext(invoiceItem));

            invoiceItem.Price = 0;

            Assert.IsFalse(Validator.TryValidateObject(invoiceItem, new ValidationContext(invoiceItem), validationResults, true));

            invoiceItem.Price = 10;
            Assert.IsTrue(Validator.TryValidateObject(invoiceItem, new ValidationContext(invoiceItem), validationResults, true));
            Assert.AreEqual(10, invoiceItem.Price);
        }

        [Test]
        public void Test_Create_InvoiceItem_VAT_Wrong_Range()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ItemDetails invoiceItem = new ItemDetails();
            invoiceItem.Item = "Test Item";
            invoiceItem.Quantity = 1;
            invoiceItem.Price = 0.2m;
            invoiceItem.VAT = 200;

            Assert.IsFalse(Validator.TryValidateObject(invoiceItem, new ValidationContext(invoiceItem), validationResults, true));

            invoiceItem.VAT = 70;
            Assert.IsTrue(Validator.TryValidateObject(invoiceItem, new ValidationContext(invoiceItem), validationResults, true));
            Assert.AreEqual(70, invoiceItem.VAT);
        }

        [Test]
        public void Test_Create_InvoiceItem_CreatedTime()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ItemDetails invoiceItem = new ItemDetails();
            invoiceItem.Item = "Test Item";
            invoiceItem.Quantity = 1;
            invoiceItem.Price = 0.2m;
            invoiceItem.VAT = 200;

            Assert.That(invoiceItem.CreatedTime.HasValue);
        }

        [Test]
        public void Test_Create_InvoiceItem_GeneratedFields()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ItemDetails invoiceItem = new ItemDetails();
            invoiceItem.Item = "Test Item";
            invoiceItem.Quantity = 1;
            invoiceItem.Price = 0.2m;
            invoiceItem.VAT = 100;

            Assert.AreEqual(1 * 0.2, invoiceItem.Total);
            Assert.AreEqual(invoiceItem.Total * (1 + invoiceItem.VAT / 100), invoiceItem.TotalPlusVAT);
            Assert.AreEqual(invoiceItem.TotalPlusVAT - invoiceItem.Total, invoiceItem.VATAmount);
        }
    }
}