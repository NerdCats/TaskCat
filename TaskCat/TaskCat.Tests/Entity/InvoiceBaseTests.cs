namespace TaskCat.Tests.Model.Invoice
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Data.Entity;
    using Data.Model.Inventory;
    using Data.Model.Payment;
    [TestFixture(TestOf = typeof(InvoiceBase))]
    public class InvoiceBaseTests
    {
        [Test()]
        public void Test_InvoiceBase_Creation()
        {
            InvoiceBase baseInvoice = new InvoiceBase();
            Assert.That(baseInvoice.CreatedTime.HasValue);
            Assert.IsNotNull(baseInvoice);
        }

        [Test()]
        public void Test_InvoiceBase_Required()
        {
            InvoiceBase baseInvoice = new InvoiceBase();
            Assert.IsFalse(Validator.TryValidateObject(baseInvoice, new ValidationContext(baseInvoice), new List<ValidationResult>(), true));

            baseInvoice.Vendor = "TestVendorId";
            baseInvoice.Notes = "Test Note";
            Assert.IsTrue(Validator.TryValidateObject(baseInvoice, new ValidationContext(baseInvoice), new List<ValidationResult>(), true));
        }

        [Test()]
        public void Test_InvoiceBase_Generated()
        {
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

            InvoiceBase baseInvoice = new InvoiceBase(invoiceItems);

            DateTime dueDate = baseInvoice.CreatedTime.Value.AddDays(5);

            baseInvoice.Vendor = "TestVendorId";
            baseInvoice.Notes = "Test Note";
            baseInvoice.ServiceCharge = 100;
            baseInvoice.DueDate = dueDate;
            baseInvoice.Paid = PaymentStatus.Pending;
            baseInvoice.NetTotal = 100;
            baseInvoice.SubTotal = 200;
            baseInvoice.TotalToPay = 400;
            baseInvoice.TotalVATAmount = 10;
            baseInvoice.Weight = 5;

            Assert.AreEqual(100, baseInvoice.NetTotal);
            Assert.AreEqual(200, baseInvoice.SubTotal);
            Assert.AreEqual(400, baseInvoice.TotalToPay);
            Assert.AreEqual(10, baseInvoice.TotalVATAmount);
            Assert.AreEqual(5, baseInvoice.Weight);
        }
    }
}