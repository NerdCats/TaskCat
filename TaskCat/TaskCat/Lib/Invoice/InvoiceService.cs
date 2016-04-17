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

        public Task<TResponse> Generate<TRequest, TResponse>()
            where TRequest : InvoiceRequestBase
            where TResponse : InvoiceBase, IInvoiceFor<TRequest>, new()
        {
            throw new NotImplementedException();
        }
    }
}