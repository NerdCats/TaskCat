namespace TaskCat.Lib.Invoice
{
    using Data.Entity;
    using System.IO;

    public interface IPDFService<TInvoice> where TInvoice: InvoiceBase
    {
        MemoryStream GeneratePDF(TInvoice invoice);
    }
}
