namespace TaskCat.Job
{
    using Common.Db;
    using System;
    using System.Threading.Tasks;

    public class LocalityService : ILocalityService
    {
        private IDbContext context;

        public LocalityService(IDbContext context)
        {
            this.context = context;
        }
        public Task RefreshLocalities()
        {
            throw new NotImplementedException();
        }
    }
}
