namespace TaskCat.Lib.Db
{
    using NLog;
    using Common.Db;
    using System.Threading.Tasks;

    public class ApiDbContext : DbContext
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ApiDbContext()
        {
            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            Task.Factory.StartNew(() =>
            {
                IndexFacade.EnsureJobIndexes(this.Jobs);
                IndexFacade.EnsureHRIDIndex(this.HRIDs);
                IndexFacade.EnsureDropPointIndex(this.DropPoints);
                IndexFacade.EnsureVendorIndex(this.Vendors);
                IndexFacade.EnsureProductCategoriesIndex(this.ProductCategories);
                IndexFacade.EnsureCommentIndexes(this.Comments);
                IndexFacade.EnsureJobActivityIndexes(this.JobActivityCollection);
            });
        }

        public void Dispose()
        {
            //TODO: Need to write this;
        }
    }
}