namespace TaskCat.Data.Lib.Invoice
{
    using System.Threading.Tasks;
    using Entity;
    using TaskCat.Lib.Invoice;

    public interface IInvoiceService
    {
        Task<TResponse> Generate<TRequest, TResponse>()
            where TRequest : InvoiceRequestBase
            where TResponse : InvoiceBase, IInvoiceFor<TRequest>, new();
    }
}