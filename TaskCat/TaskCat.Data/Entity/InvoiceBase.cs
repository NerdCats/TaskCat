namespace TaskCat.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson.Serialization.Attributes;
    using Lib.Invoice.Response;
    using Model.Inventory;
    using Model.Payment;

    [BsonKnownTypes(typeof(DeliveryInvoice))]
    public class InvoiceBase : HRIDEntity
    {
        // FIXME: Would be fixed after Vendor profile is introduced
        [Required]
        public string Vendor { get; set; }

        [Required]
        public string Notes { get; set; }
        public DateTime? CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        public PaymentStatus Paid { get; set; }

        public virtual decimal ServiceCharge { get; set; }

        public decimal TotalVATAmount { get; set; }

        public decimal SubTotal { get; set; }
        public decimal NetTotal { get; set; }

        public decimal TotalToPay { get; set; }
        public decimal Weight { get; set; }

        public virtual IEnumerable<ItemDetails> InvoiceDetails { get; set; }

        public InvoiceBase()
        {
            InvoiceDetails = new List<ItemDetails>();
        }

        public InvoiceBase(IEnumerable<ItemDetails> invoiceDetails)
        {
            InvoiceDetails = invoiceDetails;
        }
    }
}
