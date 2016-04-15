namespace TaskCat.Data.Entity.Invoice
{
    using System.Collections.Generic;
    using Model.Invoice;
    using Model.Geocoding;

    public class DeliveryInvoice : InvoiceBase
    {
        public DefaultAddress ShippingAddress { get; set; }
        public DefaultAddress BillingAddress { get; set; }
        public string CustomerName { get; set; }

        public DeliveryInvoice(ICollection<ItemDetails> invoiceDetails) 
            : base(invoiceDetails)
        {

        }
    }
}
