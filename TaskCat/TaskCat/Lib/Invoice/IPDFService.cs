namespace TaskCat.Lib.Invoice
{
    using Data.Entity;

    public interface IPDFService<TInvoice> where TInvoice: InvoiceBase
    {
        void GeneratePDF(TInvoice invoice);
    }
}
