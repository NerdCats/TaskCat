namespace TaskCat.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson.Serialization.Attributes;
    using Lib.Invoice.Response;
    using Model.Inventory;

    [BsonKnownTypes(typeof(DeliveryInvoice))]
    public class InvoiceBase : DbEntity
    {
        public int InvoiceNumber { get; set; }

        // FIXME: Would be fixed after Vendor profile is introduced
        [Required]
        public string Vendor { get; set; }

        [Required]
        public string Notes { get; set; }
        public DateTime? CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        public bool Paid { get; set; }

        public virtual decimal ServiceCharge { get; set; }

        public decimal TotalVATAmount { get; set; }

        public decimal SubTotal { get; set; }
        public decimal NetTotal { get; set; }

        public decimal TotalToPay { get; set; }
        public decimal Weight { get; set; }

        public virtual ICollection<ItemDetails> InvoiceDetails { get; set; }

        public InvoiceBase()
        {
            InvoiceDetails = new List<ItemDetails>();
        }

        public InvoiceBase(ICollection<ItemDetails> invoiceDetails)
        {
            InvoiceDetails = invoiceDetails;
        }
    }
}
