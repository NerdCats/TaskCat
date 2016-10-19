namespace TaskCat.Tests.Model.Inventory
{
    using Data.Model.Inventory;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [TestFixture(TestOf = typeof(ItemDetails))]
    public class ItemDetailsTest
    {
        [Test]
        public void Test_Create_ItemDetails()
        {
            ItemDetails itemDetails = new ItemDetails();

            Assert.NotNull(itemDetails);
        }

        [Test]
        public void Test_Create_ItemDetails_Without_Item()
        {
            Assert.Throws(typeof(ValidationException), () =>
            {
                ItemDetails itemDetails = new ItemDetails();

                Assert.NotNull(itemDetails);
                Validator.ValidateObject(itemDetails, new ValidationContext(itemDetails));
            });
        }

        [Test]
        public void Test_Create_ItemDetails_With_Item()
        {
            Assert.DoesNotThrow(() =>
            {
                ItemDetails itemDetails = new ItemDetails();
                itemDetails.Item = "Test Item";

                Assert.NotNull(itemDetails);
                Assert.NotNull(itemDetails.Item);
                Assert.AreEqual("Test Item", itemDetails.Item);

                Validator.ValidateObject(itemDetails, new ValidationContext(itemDetails));
            });
        }

        [Test]
        public void Test_Create_InvoiceItem_Quantity_WrongRange()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ItemDetails itemDetails = new ItemDetails();
            itemDetails.Item = "Test Item";
            itemDetails.Quantity = 0;
            itemDetails.Price = 10;

            Assert.IsFalse(Validator.TryValidateObject(itemDetails, new ValidationContext(itemDetails), validationResults, true));

            itemDetails.Quantity = 2;
            Assert.IsTrue(Validator.TryValidateObject(itemDetails, new ValidationContext(itemDetails), validationResults, true));
            Assert.AreEqual(2, itemDetails.Quantity);
        }

        [Test]
        public void Test_Create_InvoiceItem_Price_Wrong_Range()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ItemDetails itemDetails = new ItemDetails();
            itemDetails.Item = "Test Item";
            itemDetails.Quantity = 1;

            Validator.ValidateObject(itemDetails, new ValidationContext(itemDetails));

            itemDetails.Price = 0;

            Assert.IsFalse(Validator.TryValidateObject(itemDetails, new ValidationContext(itemDetails), validationResults, true));

            itemDetails.Price = 10;
            Assert.IsTrue(Validator.TryValidateObject(itemDetails, new ValidationContext(itemDetails), validationResults, true));
            Assert.AreEqual(10, itemDetails.Price);
        }

        [Test]
        public void Test_Create_InvoiceItem_VAT_Wrong_Range()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ItemDetails itemDetails = new ItemDetails();
            itemDetails.Item = "Test Item";
            itemDetails.Quantity = 1;
            itemDetails.Price = 0.2m;
            itemDetails.VAT = 200;

            Assert.IsFalse(Validator.TryValidateObject(itemDetails, new ValidationContext(itemDetails), validationResults, true));

            itemDetails.VAT = 70;
            Assert.IsTrue(Validator.TryValidateObject(itemDetails, new ValidationContext(itemDetails), validationResults, true));
            Assert.AreEqual(70, itemDetails.VAT);
        }

        [Test]
        public void Test_Create_InvoiceItem_CreatedTime()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ItemDetails itemDetails = new ItemDetails();
            itemDetails.Item = "Test Item";
            itemDetails.Quantity = 1;
            itemDetails.Price = 0.2m;
            itemDetails.VAT = 200;

            Assert.That(itemDetails.CreatedTime.HasValue);
        }

        [Test]
        public void Test_Create_InvoiceItem_GeneratedFields()
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ItemDetails itemDetails = new ItemDetails();
            itemDetails.Item = "Test Item";
            itemDetails.Quantity = 1;
            itemDetails.Price = 0.2m;
            itemDetails.VAT = 100;

            Assert.AreEqual(1 * 0.2, itemDetails.Total);
            Assert.AreEqual(itemDetails.Total * (1 + itemDetails.VAT / 100), itemDetails.TotalPlusVAT);
            Assert.AreEqual(itemDetails.TotalPlusVAT - itemDetails.Total, itemDetails.VATAmount);
        }
    }
}