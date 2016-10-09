namespace TaskCat.Lib.Invoice.Request
{
    using Data.Model.Geocoding;
    using System.Collections.Generic;
    using Data.Model.Inventory;

    public class ItemDetailsInvoiceRequest : InvoiceRequestBase
    {
        public DefaultAddress DeliveryFrom { get; set; }
        public DefaultAddress DeliveryTo { get; set; }
        public string VendorName { get; set; }
        public DefaultAddress VendorAddress { get; set; }
        public IEnumerable<ItemDetails> ItemDetails { get; set; }
        public string NotesToDeliveryMan { get; set; }
    }

}