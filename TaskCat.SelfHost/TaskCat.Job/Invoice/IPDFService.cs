namespace TaskCat.Job.Invoice
{
    using Data.Lib.Invoice;
    using System.IO;

    public interface IPDFService<TInvoice> where TInvoice: InvoiceBase
    {
        MemoryStream GeneratePDF(TInvoice invoice);
    }
}
