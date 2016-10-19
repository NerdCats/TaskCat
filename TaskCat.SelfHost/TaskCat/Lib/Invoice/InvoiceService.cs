namespace TaskCat.Lib.Invoice
{
    using Data.Lib.Invoice;
    using Data.Entity;

    internal class InvoiceService : IInvoiceService
    {
        public InvoiceService()
        {
        }

        public TResponse GenerateInvoice<TRequest, TResponse>(TRequest request)
            where TRequest : InvoiceRequestBase
            where TResponse : InvoiceBase, IInvoiceFor<TRequest>, new()
        {
            var response = new TResponse();
            response.Populate(request);
            return response;
        }
    }
}