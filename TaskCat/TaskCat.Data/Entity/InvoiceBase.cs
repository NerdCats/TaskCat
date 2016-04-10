namespace TaskCat.Data.Model
{
    using System;
    using Entity;
    using System.Collections.Generic;
    using Invoice;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;

    public class InvoiceBase : DbEntity
    {
        public int InvoiceNumber { get; set; }
        public string VendorId { get; set; }

        [Required]
        public string Notes { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime DueDate { get; set; }

        public bool Paid { get; set; }

        public virtual decimal ServiceCharge { get; set; }

        public decimal VATAmount
        {
            get
            {
                return this.SubTotal - this.NetTotal;
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

        public virtual ICollection<InvoiceItem> InvoiceDetails { get; set; }

        public InvoiceBase()
        {
            InvoiceDetails = new List<InvoiceItem>();
        }

        public InvoiceBase(ICollection<InvoiceItem> invoiceDetails)
        {
            InvoiceDetails = invoiceDetails;
        }
    }
}
