namespace TaskCat.Job.Invoice
{
    using Data.Lib.Invoice;

    public interface IInvoiceService
    {
        TResponse GenerateInvoice<TRequest, TResponse>(TRequest request)
            where TRequest : InvoiceRequestBase
            where TResponse : InvoiceBase, IInvoiceFor<TRequest>, new();

    }
}