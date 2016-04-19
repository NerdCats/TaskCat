namespace TaskCat.Data.Model.Tests.Model.Invoice
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data.Model.Invoice;
    using System.Linq;

    using Entity;
    using Inventory;
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
            baseInvoice.Paid = false;

            Assert.AreEqual(invoiceItems.Sum(i => i.Total), baseInvoice.NetTotal);
            Assert.AreEqual(invoiceItems.Sum(i => i.TotalPlusVAT), baseInvoice.SubTotal);
            Assert.AreEqual(100 + invoiceItems.Sum(i => i.TotalPlusVAT), baseInvoice.TotalToPay);
            Assert.AreEqual(invoiceItems.Sum(i => i.TotalPlusVAT) - invoiceItems.Sum(i => i.Total), baseInvoice.VATAmount);
            Assert.AreEqual(invoiceItems.Sum(x=>x.Weight), baseInvoice.Weight);
        }
    }
}