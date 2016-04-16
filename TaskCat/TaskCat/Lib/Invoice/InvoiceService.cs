namespace TaskCat.Lib.Invoice
{
    using Db;
    using System;
    using System.Threading.Tasks;

    internal class InvoiceService : IInvoiceService
    {
        IDbContext _dbContext;
        public InvoiceService(IDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public async Task<TResponse> Generate<TRequest, TResponse>()
            where TRequest : InvoiceRequestBase
            where TResponse : IInvoiceFor<TRequest>, new()
        {
            throw new NotImplementedException();
        }
    }
}