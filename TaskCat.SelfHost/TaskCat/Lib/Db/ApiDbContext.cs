namespace TaskCat.Lib.Db
{
    using NLog;
    using Common.Db;

    public class ApiDbContext : DbContext
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        public ApiDbContext()
        {
            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            IndexFacade.EnsureJobIndexes(this.Jobs);
            IndexFacade.EnsureHRIDIndex(this.HRIDs);
            IndexFacade.EnsureDropPointIndex(this.DropPoints);
            IndexFacade.EnsureVendorIndex(this.Vendors);
            IndexFacade.EnsureProductCategoriesIndex(this.ProductCategories);
            IndexFacade.EnsureComments(this.Comments);
        }

        public void Dispose()
        {
            //TODO: Need to write this;
        }
    }
}