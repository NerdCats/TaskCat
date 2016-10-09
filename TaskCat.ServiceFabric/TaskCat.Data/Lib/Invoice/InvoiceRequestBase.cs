using System;
using TaskCat.Data.Model.Geocoding;
using TaskCat.Data.Model.Payment;

namespace TaskCat.Lib.Invoice
{
    public class InvoiceRequestBase
    {
        public string CustomerName { get; set; }
        public decimal? NetTotal { get; set; }
        public decimal? TotalVATAmount { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? ServiceCharge { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? TotalToPay { get; set; }
        public DateTime? ETA { get; internal set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DefaultAddress CustomerBillingAddress { get; set; }
    }
}