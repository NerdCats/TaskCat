namespace TaskCat.Lib.Invoice
{
    using System.Threading.Tasks;

    internal interface IInvoiceService
    {
        Task<TResponse> Generate<TRequest, TResponse>()
            where TRequest : InvoiceRequestBase
            where TResponse : IInvoiceFor<TRequest>, new();
    }
}