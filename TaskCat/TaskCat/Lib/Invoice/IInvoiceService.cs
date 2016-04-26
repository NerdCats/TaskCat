namespace TaskCat.Lib.Invoice
{
    using Data.Entity;
    using Data.Lib.Invoice;

    internal interface IInvoiceService
    {
        TResponse GenerateInvoice<TRequest, TResponse>(TRequest request)
            where TRequest : InvoiceRequestBase
            where TResponse : InvoiceBase, IInvoiceFor<TRequest>, new();
    }
}