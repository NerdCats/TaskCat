namespace TaskCat.Data.Lib.Invoice
{
    using TaskCat.Lib.Invoice;

    public interface IInvoiceFor<TRequest> where TRequest : InvoiceRequestBase
    {
        void Populate(TRequest request);
    }
}