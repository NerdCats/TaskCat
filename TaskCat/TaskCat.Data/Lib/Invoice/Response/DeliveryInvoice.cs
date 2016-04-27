namespace TaskCat.Data.Lib.Invoice.Response
{
    using Model.Geocoding;
    using TaskCat.Lib.Invoice.Request;
    using System;
    using Model.Payment;

    public class DeliveryInvoice : InvoiceBase, IInvoiceFor<ItemDetailsInvoiceRequest>
    {
        public DefaultAddress ShippingAddress { get; set; }
        public DefaultAddress BillingAddress { get; set; }
        public DefaultAddress VendorAddress { get; set; }
        public string CustomerName { get; set; }

        public void Populate(ItemDetailsInvoiceRequest invoiceRequest)
        {
            if (invoiceRequest.DeliveryTo == null) throw new ArgumentNullException(nameof(invoiceRequest.DeliveryTo));
            this.ShippingAddress = invoiceRequest.DeliveryTo;
            this.BillingAddress = invoiceRequest.DeliveryTo;

            if (invoiceRequest.DeliveryFrom == null) throw new ArgumentNullException(nameof(invoiceRequest.DeliveryFrom));
            this.VendorAddress = invoiceRequest.DeliveryFrom;
            if (invoiceRequest.VendorAddress != null)
                this.VendorAddress = invoiceRequest.VendorAddress;

            if (invoiceRequest.CustomerBillingAddress != null)
                this.BillingAddress = invoiceRequest.CustomerBillingAddress;

            if (invoiceRequest.ItemDetails == null) throw new ArgumentNullException(nameof(invoiceRequest.ItemDetails));
            this.InvoiceDetails = invoiceRequest.ItemDetails;

            if (!string.IsNullOrWhiteSpace(invoiceRequest.VendorName))
                this.Vendor = invoiceRequest.VendorName;

            if (!string.IsNullOrWhiteSpace(invoiceRequest.NotesToDeliveryMan))
                this.Notes = invoiceRequest.NotesToDeliveryMan;

            this.CreatedTime = DateTime.Now;
            // INFO: By default the Due date is set to one day apart from the order creation
            this.DueDate = invoiceRequest.ETA == null ? CreatedTime.Value.AddDays(1) : invoiceRequest.ETA;

            this.Paid = PaymentStatus.Pending;

            if(invoiceRequest.ServiceCharge == null) throw new ArgumentNullException(nameof(invoiceRequest.ServiceCharge));
            this.ServiceCharge = invoiceRequest.ServiceCharge.Value;

            if (invoiceRequest.TotalVATAmount == null) throw new ArgumentNullException(nameof(invoiceRequest.TotalVATAmount));
            this.TotalVATAmount = invoiceRequest.TotalVATAmount.Value;

            if (invoiceRequest.SubTotal == null) throw new ArgumentNullException(nameof(invoiceRequest.SubTotal));
            this.SubTotal = invoiceRequest.SubTotal.Value;

            if (invoiceRequest.NetTotal == null) throw new ArgumentNullException(nameof(invoiceRequest.SubTotal));
            this.NetTotal = invoiceRequest.NetTotal.Value;

            if(invoiceRequest.TotalToPay ==null) throw new ArgumentNullException(nameof(invoiceRequest.TotalToPay));
            this.TotalToPay = invoiceRequest.TotalToPay.Value;

            this.Weight = invoiceRequest.TotalWeight.HasValue? invoiceRequest.TotalWeight.Value: 0;
        }
    }
}
