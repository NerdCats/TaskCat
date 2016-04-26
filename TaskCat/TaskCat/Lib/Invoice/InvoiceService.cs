namespace TaskCat.Lib.Invoice
{
    using Db;
    using System;
    using System.Threading.Tasks;
    using Data.Lib.Invoice;
    using Data.Entity;

    internal class InvoiceService : IInvoiceService
    {
        IDbContext _dbContext;
        public InvoiceService(IDbContext dbcontext)
        {
            _dbContext = dbcontext;
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