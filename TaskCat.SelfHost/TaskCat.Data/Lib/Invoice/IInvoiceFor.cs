namespace TaskCat.Data.Lib.Invoice
{
    public interface IInvoiceFor<TRequest> where TRequest : InvoiceRequestBase
    {
        void Populate(TRequest request);
    }
}