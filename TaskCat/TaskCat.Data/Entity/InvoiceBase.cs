namespace TaskCat.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using Model.Invoice;
    using MongoDB.Bson.Serialization.Attributes;
    using Invoice;

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

        public decimal VATAmount
        {
            get
            {
                return SubTotal - NetTotal;
            }
        }

        public decimal SubTotal
        {
            get
            {
                if (InvoiceDetails == null)
                    return 0;

                return InvoiceDetails.Sum(i => i.TotalPlusVAT);
            }
        }

        public decimal NetTotal
        {
            get
            {
                if (InvoiceDetails == null)
                    return 0;

                return InvoiceDetails.Sum(i => i.Total);
            }
        }

        public decimal TotalToPay
        {
            get
            {
                return SubTotal + ServiceCharge;
            }
        }

        private decimal weight;
        public decimal Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
            }
        }

        public virtual ICollection<ItemDetails> InvoiceDetails { get; set; }

        public InvoiceBase()
        {
            InvoiceDetails = new List<ItemDetails>();
        }

        public InvoiceBase(ICollection<ItemDetails> invoiceDetails)
        {
            InvoiceDetails = invoiceDetails;
            weight = InvoiceDetails.Sum(x => x.Weight);
        }
    }
}
