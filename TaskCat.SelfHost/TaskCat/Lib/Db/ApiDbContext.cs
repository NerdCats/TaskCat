namespace TaskCat.Lib.Db
{
    using NLog;
    using Common.Db;
    using MongoDB.Driver;
    using Data.Entity;

    public class ApiDbContext : DbContext
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IMongoCollection<JobActivity> _jobActivityCollection;
        public IMongoCollection<JobActivity> JobActivityCollection
        {
            get { return _jobActivityCollection; }
        }

        public ApiDbContext()
        {
            _jobActivityCollection = Database.GetCollection<JobActivity>(CollectionNames.JobActivityCollectionName);
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