namespace TaskCat.Lib.Job
{
    using Common.Db;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class JobActivityService
    {
        private IDbContext dbcontext;

        public JobActivityService(IDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }
    }
}
