namespace TaskCat.Data.Lib.Invoice.Response
{
    using System.Collections.Generic;
    using Model.Invoice;
    using Model.Geocoding;
    using Entity;
    using TaskCat.Lib.Invoice.Request;

    public class DeliveryInvoice : InvoiceBase, IInvoiceFor<ItemDetailsInvoiceRequest>
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
