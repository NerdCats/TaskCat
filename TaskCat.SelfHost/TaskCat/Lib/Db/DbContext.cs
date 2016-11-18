namespace TaskCat.Lib.Db
{
    using Data.Entity;
    using MongoDB.Driver;
    using NLog;
    using System.Configuration;
    using Data.Entity.Identity;
    using AspNet.Identity.MongoDB;
    using Data.Entity.ShadowCat;
    using Common.Db;

    public class DbContext : IDbContext
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private MongoClient mongoClient;
        private MongoClient shadowCatMongoClient;

        public IMongoDatabase Database { get; private set; }
        public IMongoDatabase ShadowCatDatabase { get; private set; }

        #region JobsAndOrders

        private IMongoCollection<Job> _jobs;
        public IMongoCollection<Job> Jobs
        {
            get { return _jobs; }
        }

        private IMongoCollection<SupportedOrder> _supportedOrders;
        public IMongoCollection<SupportedOrder> SupportedOrders
        {
            get
            {
                return _supportedOrders;
            }
        }
        #endregion

        #region AssetTracking

        private IMongoCollection<AssetLocation> _assetLocations;
        public IMongoCollection<AssetLocation> AssetLocations
        {
            get
            {
                return _assetLocations;
            }
        }
        #endregion

        #region HRIDS=
        private IMongoCollection<HRIDEntity> _hrids;
        public IMongoCollection<HRIDEntity> HRIDs
        {
            get
            {
                return _hrids;
            }
        }
        #endregion

        #region DropPoints
        private IMongoCollection<DropPoint> _dropPoints;
        public IMongoCollection<DropPoint> DropPoints
        {
            get
            {
                return _dropPoints;
            }
        }
        #endregion

        #region Catalog
        private IMongoCollection<Store> _stores;
        public IMongoCollection<Store> Stores
        {
            get
            {
                return _stores;
            }
        }

        private IMongoCollection<ProductCategory> _productCategories;
        public IMongoCollection<ProductCategory> ProductCategories
        {
            get
            {
                return _productCategories;
            }
        }

        private IMongoCollection<Vendor> _vendors;
        public IMongoCollection<Vendor> Vendors
        {
            get
            {
                return _vendors;
            }
        }

        private IMongoCollection<Product> _products;

        public IMongoCollection<Product> Products
        {
            get { return _products;}
        }

        #endregion

        #region Comment
        private IMongoCollection<Comment> _comments;
        public IMongoCollection<Comment> Comments
        {
            get { return _comments; }
        }
        #endregion

        public DbContext()
        {
            InitiateDatabase();
            InitiateCollections();
            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            IndexFacade.EnsureJobIndexes(_jobs);
            IndexFacade.EnsureHRIDIndex(_hrids);
            IndexFacade.EnsureDropPointIndex(_dropPoints);
            IndexFacade.EnsureVendorIndex(_vendors);
            IndexFacade.EnsureProductCategoriesIndex(_productCategories);
            IndexFacade.EnsureComments(_comments);
        }

        private void InitiateCollections()
        {
            _jobs = Database.GetCollection<Job>(CollectionNames.JobsCollectionName);
            _supportedOrders = Database.GetCollection<SupportedOrder>(CollectionNames.SupportedOrderCollectionName);
            _hrids = Database.GetCollection<HRIDEntity>(CollectionNames.HRIDCollectionName);
            _dropPoints = Database.GetCollection<DropPoint>(CollectionNames.DropPointCollectionName);

            _stores = Database.GetCollection<Store>(CollectionNames.StoreColletionName);
            _productCategories = Database.GetCollection<ProductCategory>(CollectionNames.ProductCategoryCollectionName);
            _vendors = Database.GetCollection<Vendor>(CollectionNames.VendorCollectionName);
            _products = Database.GetCollection<Product>(CollectionNames.ProductCollectionName);

            _comments = Database.GetCollection<Comment>(CollectionNames.CommentCollectionName);

            _assetLocations = ShadowCatDatabase.GetCollection<AssetLocation>(ConfigurationManager.AppSettings["ShadowCat.LocationCacheCollectionName"]);
        }

        private void InitiateDatabase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Mongo.ConnectionString"].ConnectionString;
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

            var TaskCatDBName = ConfigurationManager.AppSettings["TaskCat.DbName"];
            Database = mongoClient.GetDatabase(string.IsNullOrWhiteSpace(TaskCatDBName) ? DatabaseNames.TASKCAT_DB : TaskCatDBName);

            var shadowCatConnectionString = ConfigurationManager.ConnectionStrings["ShadowCat.ConnectionString"].ConnectionString;
            if (string.Equals(connectionString, shadowCatConnectionString))
                ShadowCatDatabase = Database;
            else
            {
                var shadowCatUrlBuilder = new MongoUrlBuilder(shadowCatConnectionString);
                shadowCatMongoClient = new MongoClient(shadowCatUrlBuilder.ToMongoUrl());
                if (shadowCatUrlBuilder.DatabaseName == "admin" || string.IsNullOrWhiteSpace(shadowCatUrlBuilder.DatabaseName))
                    //Load default shadowcat database name
                    ShadowCatDatabase = shadowCatMongoClient.GetDatabase(DatabaseNames.SHADOWCAT_DEFAULT_DB);
                else
                    ShadowCatDatabase = shadowCatMongoClient.GetDatabase(shadowCatUrlBuilder.DatabaseName);
            }
        }

        public void Dispose()
        {
            //TODO: Need to write this;
        }
    }
}