using System.Collections.Generic;

namespace TaskCat.Lib.Email.Model
{
    public class EmailInvoice
    {
        public string VAT { get; set; }
        public List<EmailInvoiceItem> Items { get; set; }
        public string ShippingDate { get; set; }
        public string ServiceCharge { get; set; }
        public string Total { get; set; }
        public string SubTotal { get; set; }
        public string ShippingAddress { get; set; }
    }
}