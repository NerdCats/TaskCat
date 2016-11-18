using MongoDB.Driver;
using System.Configuration;
using TaskCat.Data.Entity;
using TaskCat.Data.Entity.Identity;
using TaskCat.Data.Entity.ShadowCat;

namespace TaskCat.Common.Db
{
    public class DbContext: IDbContext
    {
        private MongoClient mongoClient;
        private MongoClient shadowCatMongoClient;

        public IMongoDatabase Database { get; private set; }
        public IMongoDatabase ShadowCatDatabase { get; private set; }

        #region Auth

        private IMongoCollection<User> _users;
        public IMongoCollection<User> Users
        {
            get { return _users; }
        }

        private IMongoCollection<Asset> _assets;
        public IMongoCollection<Asset> Assets
        {
            get { return _assets; }
        }

        private IMongoCollection<Role> _roles;
        public IMongoCollection<Role> Roles
        {
            get { return _roles; }
        }

        private IMongoCollection<Client> _clients;
        public IMongoCollection<Client> Clients
        {
            get { return _clients; }
        }

        private IMongoCollection<RefreshToken> _refreshTokens;
        public IMongoCollection<RefreshToken> RefreshTokens
        {
            get { return _refreshTokens; }
        }

        #endregion

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
            get { return _products; }
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
